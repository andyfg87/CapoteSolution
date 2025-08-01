﻿using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class CopiersController : AbstractEntityManagementController<Copier, string, CopierInputVM, CopierDisplayVM>
    {
        private readonly IEntityRepository<MachineModel, Guid> _machineModelRepo;
        private readonly IEntityRepository<Customer, Guid> _customerRepo;
        private readonly IEntityRepository<Brand, Guid> _brandRepo;

        public CopiersController(
            IEntityRepository<Copier, string> repository,
            IEntityRepository<MachineModel, Guid> machineModelRepo,
            IEntityRepository<Brand, Guid> brandRepo,
            IEntityRepository<Customer, Guid> customerRepo,
            IStringLocalizer<CopiersController> localizer,
            ILogger<CopiersController> logger)
            : base(repository, localizer, logger)
        {
            _machineModelRepo = machineModelRepo;
            _customerRepo = customerRepo;
            _brandRepo = brandRepo;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var query = await _repository.GetAllWithNestedInclude(
                nameof(MachineModel),
                nameof(Customer),
                nameof(MachineModel) + "." + nameof(Toner),
                nameof(MachineModel) + "." + nameof(Brand));

            var paginatedData = await GetPaginatedData(query, pageNumber, pageSize);
            return View(paginatedData);
        }

        public override async Task<IActionResult> Create()
        {
            var model = new CopierInputVM
            {
                AvailableBrands = await GetBrands(),
                AvailableMachineModels = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Seleccione una marca primero" }
                }, "Value", "Text"),
                AvailableCustomers = await GetCustomers()
            };
            return View(model);
        }

        public override async Task<IActionResult> Create(CopierInputVM inputViewModel)
        {
            if (!ModelState.IsValid)
                return View(inputViewModel);

            try
            {
                var entity = inputViewModel.Export();
                entity.Id = inputViewModel.Id;
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al crear entidad");
                ModelState.AddModelError("", _localizer["ErrorCreationMessage"]);
                return View(inputViewModel);
            }
        }

        public override async Task<IActionResult> Edit(string key)
        {
            var entity = await _repository.GetAllWithNestedInclude(
                nameof(MachineModel),
                nameof(Customer),
                nameof(MachineModel) + "." + nameof(Toner),
                nameof(MachineModel) + "." + nameof(Brand))
                .Result.FirstAsync(c => c.Id == key);

            if (entity != null)
            {

                var copierInputVM = new CopierInputVM();
                copierInputVM.Import(entity);
                copierInputVM.AvailableMachineModels = await GetMachineModels();
                copierInputVM.AvailableBrands = await GetBrands();
                copierInputVM.AvailableCustomers = await GetCustomers();
                return View(copierInputVM);
            }

            return NotFound();

        }

        [HttpPost]
        public override Task<IActionResult> EditWithKeyAndModel(string id, CopierInputVM inputModel)
        {
            return base.Edit(inputModel);
        }

        [HttpGet]
        public async Task<IActionResult> ContractDetails(string id)
        {
            var contract = await _repository.GetAll().Result
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }


        public override async Task<IActionResult> Delete(string key)
        {
            var result = await _repository.GetAllWithNestedInclude(nameof(MachineModel));
            var entity = result.FirstAsync(c => c.Id == key).Result;

            if (entity == null)
                return NotFound();

            var viewModel = new CopierDisplayVM();
            viewModel.Import(entity);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineModelsByBrand(Guid brandId, Guid machineModelId)
        {
            if (brandId == Guid.Empty)
            {
                return Json(new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione una marca válida" }
        });
            }

            var machineModels = await _machineModelRepo.GetAll().Result
                .Include(mm => mm.Brand)
                .Where(mm => mm.BrandId == brandId)
                .Select(mm => new SelectListItem
                {
                    Value = mm.Id.ToString(),
                    Text = $"{mm.Brand.Name} - {mm.Name}",
                    Selected = mm.Id == machineModelId
                })
                .ToListAsync();

            if (!machineModels.Any())
            {
                machineModels = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "No hay modelos para esta marca" }
        };
            }

            return Json(machineModels);
        }

        public async Task<SelectList> GetBrands()
        {
            var brands = await _brandRepo.GetAll().Result
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToListAsync();

            var selectListBrands = new SelectList(brands, "Value", "Text");
            return selectListBrands;
        }

        private async Task<SelectList> GetMachineModels(string IdBrand = "")
        {
            var machineModels = await _machineModelRepo.GetAll().Result
                .Include(mm => mm.Brand)
                .Where(mm => mm.BrandId.ToString() == IdBrand)
                .Select(mm => new
                {
                    mm.Id,
                    DisplayText = $"{mm.Brand.Name} - {mm.Name}"
                })
                .ToListAsync();

            return new SelectList(machineModels, "Id", "DisplayText");
        }

        private async Task<SelectList> GetCustomers()
        {
            var customers = await _customerRepo.GetAll().Result
                .Select(c => new { c.Id, DisplayText = c.CustomerName })
                .ToListAsync();

            return new SelectList(customers, "Id", "DisplayText");
        }
    }
}

using CapoteSolution.Models.Entities;
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
    //[Authorize(Roles = "Admin,Manager,Technician")]
    public class CopiersController : AbstractEntityManagementController<Copier, string, CopierInputVM, CopierDisplayVM>
    {
        private readonly IEntityRepository<MachineModel, Guid> _machineModelRepo;
        private readonly IEntityRepository<Contract, Guid> _contractRepo;

        public CopiersController(
            IEntityRepository<Copier, string> repository,
            IEntityRepository<MachineModel, Guid> machineModelRepo,
            IEntityRepository<Contract, Guid> contractRepo,
            IStringLocalizer<CopiersController> localizer,
            ILogger<CopiersController> logger)
            : base(repository, localizer, logger)
        {
            _machineModelRepo = machineModelRepo;
            _contractRepo = contractRepo;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var query = await _repository.GetAllWithNestedInclude(
                nameof(MachineModel));

            var paginatedData = await GetPaginatedData(query, pageNumber, pageSize);
            return View(paginatedData);
        }

        public override async Task<IActionResult> Create()
        {
            var model = new CopierInputVM
            {
                AvailableMachineModels = await GetMachineModels()
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

        public override async Task<IActionResult> Edit(string id)
        {
            var model = await base.Edit(id) as ViewResult;
            var copierInputVM = model.Model as CopierInputVM;
            copierInputVM.AvailableMachineModels = await GetMachineModels();
            return View(copierInputVM);
        }

        [HttpPost]
        public override Task<IActionResult> Edit(string id,CopierInputVM inputModel)
        {
            return base.Edit(inputModel);
        }        

        [HttpGet]
        public async Task<IActionResult> ContractDetails(string id)
        {
            var contract = await _contractRepo.GetAll().Result
                .Include(c => c.Copier)
                .FirstOrDefaultAsync(c => c.CopierId == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        
        public override async Task<IActionResult> Delete(string key)
        {
            var result =await _repository.GetAllWithNestedInclude(nameof(MachineModel));
            var entity = result.FirstAsync(c => c.Id == key).Result;

            if(entity == null)
                return NotFound();

            var viewModel = new CopierDisplayVM();
            viewModel.Import(entity);

            return View(viewModel);
        }

        private async Task<SelectList> GetMachineModels()
        {
            var machineModels = await _machineModelRepo.GetAll().Result
                .Include(mm => mm.Brand)
                .Select(mm => new {
                    mm.Id,
                    DisplayText = $"{mm.Brand.Name} - {mm.Name}"
                })
                .ToListAsync();

            return new SelectList(machineModels, "Id", "DisplayText");
        }
    }
}

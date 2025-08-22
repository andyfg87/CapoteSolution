using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class ServicesController : AbstractEntityManagementController<Service, Guid, ServiceInputVM, ServiceDisplayVM>
    {
        private readonly IEntityRepository<Copier, string> _copierRepo;
        private readonly IEntityRepository<User, Guid> _userRepo;
        private readonly IAppLogger _logger;

        public ServicesController(IEntityRepository<Service, Guid> repository, IEntityRepository<Copier, string> copierRepo,
        IEntityRepository<User, Guid> userRepo, IStringLocalizer<ServicesController> localizer, IAppLogger logger) : base(repository, localizer, logger)
        {
            _copierRepo = copierRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            var services = _repository.GetAllWithNestedInclude(
                "Technician",
                nameof(ServiceReason),
                nameof(Copier),
                nameof(Copier)+"."+nameof(Customer));

            var paginatedData = await GetPaginatedData(services.Result, pageNumber, pageSize);
            return View(paginatedData);
        }



        public async Task<IActionResult> CreateService(string copierId = "", bool IsDetail = false)
        {
            var model = new ServiceInputVM
            {
                ServiceDate = DateTime.Now,
                CopierId = copierId,
                AvailableCopiers = await GetActiveContracts(),
                AvailableTechnicians = await GetTechnicians(),
                ServiceReasons = GetServiceReasons()// TODO Enumerable con 3 motivos
            };

            ViewBag.IsDetail = IsDetail;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRedirectCopierDetails(ServiceInputVM inputViewModel)
        {
            if (!ModelState.IsValid)
                return View(inputViewModel);

            try
            {
                var entity = inputViewModel.Export();
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                await _logger.LogInformation($"Se creó {nameof(Service)}", nameof(ServicesController), nameof(Create), JsonSerializer.Serialize(inputViewModel));
               

                return RedirectToAction("DetailsCopierByServicePagination", "Copiers", new { key = inputViewModel.CopierId });
            }
            catch (Exception ex)
            {

                _logger.LogError("Error al crear entidad", ex, ex.Message, nameof(Create), "");
                ModelState.AddModelError("", _localizer["ErrorCreationMessage"]);
                return View(inputViewModel);
            }
        }

        public override async Task<IActionResult> Edit(Guid key)
        {
            var entity = _repository.GetByIdAsync(key).Result;
            var model = new ServiceInputVM
            {
                ServiceDate = DateTime.Now,
                AvailableCopiers = await GetActiveContracts(),
                AvailableTechnicians = await GetTechnicians(),
                ServiceReasons = GetServiceReasons()// TODO Enumerable con 3 motivos
            };

            model.Import(entity);

            ViewBag.IsDetail = false;

            return View(model);
        }

        public async Task<IActionResult> EditService(Guid key,bool IsDetail = false)
        {
            var entity = _repository.GetByIdAsync(key).Result;
            var model = new ServiceInputVM
            {
                ServiceDate = DateTime.Now,
                AvailableCopiers = await GetActiveContracts(),
                AvailableTechnicians = await GetTechnicians(),
                ServiceReasons = GetServiceReasons()// TODO Enumerable con 3 motivos
            };

            model.Import(entity);
            ViewBag.IsDetail = IsDetail;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRedirectCopierDetails(ServiceInputVM inputVM)
        {
            if (!ModelState.IsValid)
                return View(inputVM);

            try
            {

                var entity = await _repository.GetByIdAsync(inputVM.Id);
                inputVM.Merge(entity);
                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation($"Se creó {nameof(Service)}", nameof(ServicesController), nameof(EditRedirectCopierDetails), JsonSerializer.Serialize(inputVM));

                return RedirectToAction("DetailsCopierByServicePagination", "Copiers", new { key = inputVM.CopierId });
            }
            catch (Exception ex)
            {

                _logger.LogError("Error al actualizar entidad", ex, ex.Message, nameof(Edit), "");
                ModelState.AddModelError("", _localizer["ErrorUpdateMessage"]);
                return View(inputVM);
            }
        }

        public override async Task<IActionResult> Delete(Guid key)
        {
            var entity = await _repository.GetAllWithNestedInclude(nameof(ServiceReason)).Result.FirstAsync(s => s.Id == key);
            var viewModel = new ServiceDisplayVM();

            if (entity == null)
            {
                return NotFound();
            }

            viewModel.Import(entity);

            return View(viewModel);

        }

        public override async Task<IActionResult> Details(Guid key)
        {
            var entity = await _repository.GetAllWithNestedInclude("Technician",
                nameof(ServiceReason),
                nameof(Copier),
                nameof(Copier) + "." + nameof(Customer)).
                Result.FirstAsync(s => s.Id == key);

            var viewModel = new ServiceDisplayVM();
            viewModel.Import(entity);

            return View(viewModel);
        }

        private async Task<SelectList> GetActiveContracts()
        {
            var contracts = await _copierRepo.GetAll().Result
                .Where(c => c.Status == ContractStatus.Active)
                .Select(c => new { c.Id, DisplayText = $"{c.Id} - {c.MachineModel.Name}" })
                .ToListAsync();

            return new SelectList(contracts, "Id", "DisplayText");
        }

        private async Task<SelectList> GetTechnicians()
        {
            var technicians = await _userRepo.GetAll().Result
                //.Where(u => u.Role == UserRole.Technician)
                .Select(u => new { u.Id, DisplayText = $"{u.FirstName} - {u.LastName}" })
                .ToListAsync();

            return new SelectList(technicians, "Id", "DisplayText");
        }

        private SelectList GetServiceReasons()
        {
            List<ServiceReason> elements = new List<ServiceReason>();
            elements.Add(new ServiceReason { Id = 1, Name = ServiceReason.Reasons.TonerChange });
            elements.Add(new ServiceReason { Id = 2, Name = ServiceReason.Reasons.MonthlyCounter });
            elements.Add(new ServiceReason { Id = 3, Name = ServiceReason.Reasons.Maintenance });

            var serviceReason = elements.Select(e => new { e.Id, DisplayText = e.Name });

            return new SelectList(serviceReason, "Id", "DisplayText");

        }
    }
}

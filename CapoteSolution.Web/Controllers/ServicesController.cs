using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class ServicesController : AbstractEntityManagementController<Service, Guid, ServiceInputVM, ServiceDisplayVM>
    {
        private readonly IEntityRepository<Copier, string> _copierRepo;
        private readonly IEntityRepository<User, Guid> _userRepo;

        public ServicesController(IEntityRepository<Service, Guid> repository, IEntityRepository<Copier, string> copierRepo,
        IEntityRepository<User, Guid> userRepo, IStringLocalizer<ServicesController> localizer, ILogger<ServicesController> logger) : base(repository, localizer, logger)
        {
            _copierRepo = copierRepo;
            _userRepo = userRepo;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var services = _repository.GetAllWithNestedInclude(
                "Technician",
                nameof(ServiceReason),
                nameof(Copier),
                nameof(Copier)+"."+nameof(Customer));

            var paginatedData = await GetPaginatedData(services.Result, pageNumber, pageSize);
            return View(paginatedData);
        }



        public async Task<IActionResult> CreateService(string copierId = "")
        {
            var model = new ServiceInputVM
            {
                ServiceDate = DateTime.Now,
                CopierId = copierId,
                AvailableCopiers = await GetActiveContracts(),
                AvailableTechnicians = await GetTechnicians(),
                ServiceReasons = GetServiceReasons()// TODO Enumerable con 3 motivos
            };
            return View(model);
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

            return View(model);
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
                .Select(c => new { c.Id, DisplayText = $"{c.SerialNumber} - {c.MachineModel.Name}" })
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

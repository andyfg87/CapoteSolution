using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    public class ServicesController : AbstractEntityManagementController<Service, Guid, ServiceInputVM, ServiceDisplayVM>
    {
        private readonly IEntityRepository<Contract, Guid> _contractRepo;
        private readonly IEntityRepository<User, Guid> _userRepo;

        public ServicesController(IEntityRepository<Service, Guid> repository, IEntityRepository<Contract, Guid> contractRepo,
        IEntityRepository<User, Guid> userRepo, IStringLocalizer<ServicesController> localizer, ILogger<ServicesController> logger) : base(repository, localizer, logger)
        {
            _contractRepo = contractRepo;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public override async Task<IActionResult> Create()
        {
            var model = new ServiceInputVM
            {
                ServiceDate = DateTime.Now,
                AvailableContracts = await GetActiveContracts(),
                AvailableTechnicians = await GetTechnicians(),
                ServiceReasons = GetServiceReasons()// TODO Enumerable con 3 motivos
            };
            return View(model);
        }

        private async Task<SelectList> GetActiveContracts()
        {
            var contracts = await _contractRepo.GetAll().Result
                .Where(c => c.Status == ContractStatus.Active)
                .Select(c => new { c.Id, DisplayText = $"{c.Copier.SerialNumber} - {c.Copier.MachineModel.Name}" })
                .ToListAsync();

            return new SelectList(contracts, "Id", "DisplayText");
        }

        private async Task<SelectList> GetTechnicians()
        {
            var technicians = await _userRepo.GetAll().Result
                .Where(u => u.Role == UserRole.Technician)
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

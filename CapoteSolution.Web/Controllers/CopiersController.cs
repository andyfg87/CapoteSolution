using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CapoteSolution.Web.Controllers
{
    //[Authorize(Roles = "Admin,Manager,Technician")]
    public class CopiersController : AbstractEntityManagementController<Copier, Guid, CopierInputVM, CopierDisplayVM>
    {
        private readonly IEntityRepository<MachineModel, Guid> _machineModelRepo;
        private readonly IEntityRepository<Contract, Guid> _contractRepo;

        public CopiersController(
            IEntityRepository<Copier, Guid> repository,
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
            var copiers = await _repository.GetAll();
            var   cops = copiers.Include(c => c.MachineModel)
                    .ThenInclude(mm => mm.Brand)
            .Include(c => c.Contract)
            .OrderBy(c => c.SerialNumber)
            .Select(c=>c);

            var paginatedData = GetPaginatedData(copiers, pageNumber, pageSize);           

            return View(paginatedData.Result);
        }

        public override async Task<IActionResult> Create()
        {
            var model = new CopierInputVM
            {
                AvailableMachineModels = await GetMachineModels()
            };
            return View(model);
        }

        public override async Task<IActionResult> Edit(Guid id)
        {
            var model = await base.Edit(id) as ViewResult;
            var copierInputVM = model.Model as CopierInputVM;
            copierInputVM.AvailableMachineModels = await GetMachineModels();
            return View(copierInputVM);
        }

        [HttpGet]
        public async Task<IActionResult> ContractDetails(Guid id)
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

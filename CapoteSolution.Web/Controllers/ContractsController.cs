using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    public class ContractsController : AbstractEntityManagementController<Contract, Guid, ContractInputVM, ContractDisplayVM>
    {
        private readonly IEntityRepository<Copier, Guid> _copierRepo;

        public ContractsController(IEntityRepository<Contract, Guid> repository, IEntityRepository<Copier, Guid> copierRepo, IStringLocalizer<ContractsController> localizer, ILogger<ContractsController> logger) : base(repository, localizer, logger)
        {
            this._copierRepo = copierRepo;
        }

        public override async  Task<IActionResult> Create()
        {
            var model = new ContractInputVM
            {
                AvailableCopiers = await GetAvailableCopiers()
            };
            return View(model);
        }

        private async Task<SelectList> GetAvailableCopiers()
        {
            var copiers = await _copierRepo.GetAll().Result
                .Where(c => c.Contract == null) //Solo las impersoras sin contrato
                .Select(c => new { c.Id, DisplayText = $"{c.SerialNumber} ({c.MachineModel.Name})" })
                .ToListAsync();

            return new SelectList(copiers, "Id", "DisplayText");
        }
    }
}

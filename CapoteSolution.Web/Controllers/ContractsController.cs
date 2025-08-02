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
    public class ContractsController : AbstractEntityManagementController<Contract, Guid, ContractInputVM, ContractDisplayVM>
    {
        private readonly IEntityRepository<Copier, string> _copierRepo;
        private readonly IEntityRepository<Customer, System.Guid> _customerRepo;

        public ContractsController(IEntityRepository<Contract, Guid> repository, IEntityRepository<Copier, string> copierRepo, IEntityRepository<Customer, System.Guid> customerRepo, IStringLocalizer<ContractsController> localizer, ILogger<ContractsController> logger) : base(repository, localizer, logger)
        {
            this._copierRepo = copierRepo;
            this._customerRepo = customerRepo;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var contracts = _repository.GetAllWithNestedInclude(
                nameof(Customer),
                nameof(Copier)+"."+nameof(MachineModel));

            var paginatedData = await GetPaginatedData(contracts.Result, pageNumber, pageSize);
            return View(paginatedData);
        }

        /*public override async  Task<IActionResult> Create()
        {
            var model = new ContractInputVM
            {
                AvailableCopiers = await GetAvailableCopiers(),
                AvailableCustomers = await GetAvailableCustomers()
            };
            return View(model);
        }*/

       /* public override async Task<IActionResult> Edit(Guid key)
        {
            var entity = _repository.GetByIdAsync(key).Result;
            if (entity == null)
                return NotFound();
            var model = new ContractInputVM();
            model.AvailableCopiers = await GetAvailableCopiers(entity.CopierId.ToString());
            model.AvailableCustomers = await GetAvailableCustomers(entity.CustomerId.ToString());
            model.Import(entity);

            return View(model);
        }*/

        [HttpGet]
        public IActionResult Details(Guid key)
        {
            var contract = _repository.GetAllWithNestedInclude(nameof(Copier),
                nameof(Customer)).Result.First(c => c.Id == key);

            var viewModel = new ContractDisplayVM();
            viewModel.Import(contract);

            return View(viewModel);
        }

        public override async Task<IActionResult> Delete(Guid key)
        {
            var entity = _repository.GetAllWithNestedInclude(
                nameof(Copier),
                nameof(Copier)+"."+nameof(MachineModel),
                nameof(Customer)).Result.FirstAsync(c => c.Id == key);

            var viewModel = new ContractDisplayVM();
            viewModel.Import(entity.Result);

            return View(viewModel);
        }


       /* private async Task<SelectList> GetAvailableCopiers()
        {
            var copiers = await _copierRepo.GetAll().Result
                .Where(c => c.Contract == null) //Solo las impersoras sin contrato
                .Select(c => new {  c.Id, DisplayText = $"{c.SerialNumber} ({c.MachineModel.Name})" })
                .ToListAsync();

            return new SelectList(copiers, "Id", "DisplayText");
        }*/

        /*private async Task<SelectList> GetAvailableCustomers()
        {
            var customers = await _customerRepo.GetAll().Result
                .Where(c => c.Contract == null) //Solo las impersoras sin contrato
                .Select(c => new { c.Id, DisplayText = $"{c.CustomerName}" })
                .ToListAsync();

            return new SelectList(customers, "Id", "DisplayText");
        }

        private async Task<SelectList> GetAvailableCopiers(string CopierId = "")
        {
            var copiers = await _copierRepo.GetAll().Result                
                .Select(c => new { c.Id, DisplayText = $"{c.SerialNumber} ({c.MachineModel.Name})" })
                .ToListAsync();

            return new SelectList(copiers, "Id", "DisplayText");
        }

        private async Task<SelectList> GetAvailableCustomers(string CustomerId = "")
        {
            var customers = await _customerRepo.GetAll().Result                
                .Select(c => new { c.Id, DisplayText = $"{c.CustomerName}" })
                .ToListAsync();

            return new SelectList(customers, "Id", "DisplayText");
        }*/
    }
}

using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using CapoteSolution.Web.Paginations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class CustomersController : AbstractEntityManagementController<Customer, System.Guid, CustomerInputVM, CustomerDisplayVM>
    {    
        private readonly IEntityRepository<Copier, string> _copierRepo;
        private readonly IAppLogger _logger;

        public CustomersController(IEntityRepository<Customer, Guid> repository, IEntityRepository<Copier, string> copierRepo, IStringLocalizer<CustomersController> localizer, IAppLogger logger) : base(repository, localizer, logger)
        {
            this._copierRepo = copierRepo;
            _logger = logger;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            var customers = _repository.GetAllWithNestedInclude("Copiers");

            var paginatedData = await GetPaginatedData(customers.Result, pageNumber, pageSize);

            return View(paginatedData);
        }

        public override async Task<IActionResult> Details(Guid key)
        {
            var entity = await _repository.GetAllWithNestedInclude("Copiers").Result.FirstAsync(c => c.Id == key);

            var viewModel = new CustomerDisplayVM();
            viewModel.Import(entity);

            ViewBag.Copiers = entity.Copiers;

            return View(viewModel);
        }
        
        public async Task<IActionResult> DetailByCopiers(Guid key, int page = 1, int pageSize = 5)
        {
            var entity = await _repository.GetAllWithNestedInclude($"Copiers.{nameof(MachineModel)}.{nameof(Toner)}",
                $"Copiers.Services",
                $"Copiers.Services.{nameof(ServiceReason)}").Result.FirstAsync(c => c.Id == key);

            var viewModel = new CustomerDisplayVM();
            viewModel.Import(entity);           

            ViewBag.Copiers = PaginatedList<Copier>.Create(entity.Copiers, page, pageSize);

            return View(viewModel);
        }
    }
}

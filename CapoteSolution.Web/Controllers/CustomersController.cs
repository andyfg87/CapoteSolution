using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class CustomersController : AbstractEntityManagementController<Customer, System.Guid, CustomerInputVM, CustomerDisplayVM>
    {
        public CustomersController(IEntityRepository<Customer, Guid> repository, IStringLocalizer<CustomersController> localizer, ILogger<CustomersController> logger) : base(repository, localizer, logger)
        {
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var customers = _repository.GetAllWithNestedInclude(nameof(Contract));

            var paginatedData = await GetPaginatedData(customers.Result, pageNumber, pageSize);

            return View(paginatedData);
        }
    }
}

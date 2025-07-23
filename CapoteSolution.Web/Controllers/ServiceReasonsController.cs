using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceReasonsController : AbstractEntityManagementController<ServiceReason, byte, ServiceReasonInputVM, ServiceReasonDisplayVM>
    {
        public ServiceReasonsController(IEntityRepository<ServiceReason, byte> repository, IStringLocalizer<ServiceReasonsController> localizer, ILogger<ServiceReasonsController> logger) : base(repository, localizer, logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

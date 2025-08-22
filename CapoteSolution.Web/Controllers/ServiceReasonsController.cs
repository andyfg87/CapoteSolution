using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
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
        private IAppLogger _logger;
        public ServiceReasonsController(IEntityRepository<ServiceReason, byte> repository, IStringLocalizer<ServiceReasonsController> localizer, IAppLogger logger) : base(repository, localizer, logger)
        {
            _logger = logger;
        }
        
    }
}

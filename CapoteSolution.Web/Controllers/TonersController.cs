using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class TonersController : AbstractEntityManagementController<Toner, Guid, TonerInputVM, TonerDisplayVM>
    {
        private readonly IAppLogger _logger;
        public TonersController(IEntityRepository<Toner, Guid> repository, IStringLocalizer<TonersController> localizer, IAppLogger logger) : base(repository, localizer, logger)
        {
            _logger = logger;
        }
       
    }
}

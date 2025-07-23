using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    public class TonersController : AbstractEntityManagementController<Toner, Guid, TonerInputVM, TonerDisplayVM>
    {
        public TonersController(IEntityRepository<Toner, Guid> repository, IStringLocalizer<TonersController> localizer, ILogger<TonersController> logger) : base(repository, localizer, logger)
        {
        }
       
    }
}

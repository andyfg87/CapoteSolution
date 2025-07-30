using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class BrandsController : AbstractEntityManagementController<Brand, Guid, BrandInputVM, BrandDisplayVM>
    {
        public BrandsController(IEntityRepository<Brand, Guid> repository, IStringLocalizer<BrandsController> localizer, ILogger<BrandsController> logger) : base(repository, localizer, logger)
        {
        }
               
    }
}

using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class MachineModelsController : AbstractEntityManagementController<MachineModel, Guid, MachineModelInputVM, MachineModelDisplayVM>
    {
        private readonly IEntityRepository<Brand, Guid> _brandRepo;
        private readonly IEntityRepository<Toner, Guid> _tonerRepo;

        public MachineModelsController(IEntityRepository<MachineModel, Guid> repository, IEntityRepository<Brand, Guid> brandRepo,
        IEntityRepository<Toner, Guid> tonerRepo, IStringLocalizer<MachineModelsController> localizer, ILogger<MachineModelsController> logger) : base(repository, localizer, logger)
        {
            _brandRepo = brandRepo;
            _tonerRepo = tonerRepo;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var includes = new List<Expression<Func<MachineModel, object>>>
            {
                mm => mm.Brand,
                mm => mm.Toner
            };

            var query = (await _repository.GetAllWithInclude(includes: includes)).AsQueryable();
            var paginatedData = await GetPaginatedData(query, pageNumber, pageSize);
            return View(paginatedData);
        }

        public override async Task<IActionResult> Create()
        {
            var model = new MachineModelInputVM
            {
                Brands = await GetBrands(),
                Toners = await GetToners()
            };
            return View(model);
        }

        public override async Task<IActionResult> Edit(Guid key)
        {
            var entity = await _repository.GetByIdAsync(key);
            if (entity == null) 
            {
                return NotFound();
            }

            var viewModel = new MachineModelInputVM();
            viewModel.Toners = await GetToners();
            viewModel.Brands = await GetBrands();
            viewModel.Import(entity);
            return View(viewModel);
        }


        private async Task<SelectList> GetBrands()
        {
            var brands = await _brandRepo.GetAll().Result
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();
            return new SelectList(brands, "Id", "Name");
        }

        private async Task<SelectList> GetToners()
        {
            var toners = await _tonerRepo.GetAll().Result
                .Select(t => new { t.Id, t.Model })
                .ToListAsync();
            return new SelectList(toners, "Id", "Model");
        }
    }
}

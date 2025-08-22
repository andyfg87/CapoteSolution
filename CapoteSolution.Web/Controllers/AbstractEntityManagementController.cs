using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Paginations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace CapoteSolution.Web.Controllers
{
    public class AbstractEntityManagementController<TEntity, TKey, TInputViewModel, TDisplayViewModel>: Controller 
        where TEntity: class, IEntity<TKey>
        where TInputViewModel: IEntityInputModel<TEntity, TKey>, new()
        where TDisplayViewModel: IEntityDisplayModel<TEntity, TKey>, new()
    {
        protected readonly IEntityRepository<TEntity, TKey> _repository;
        protected readonly IStringLocalizer _localizer;
        protected readonly IAppLogger _logger;

        public AbstractEntityManagementController(
        IEntityRepository<TEntity, TKey> repository,
        IStringLocalizer localizer,
        IAppLogger logger)
        {
            _repository = repository;
            _localizer = localizer;
            _logger = logger;
        }        

        public virtual async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            var query = _repository.GetAll();
            var paginatedData = await GetPaginatedData(query.Result, pageNumber, pageSize);
            return View(paginatedData);
        }

        public virtual async Task<IActionResult> Create()
        {
            return View(new TInputViewModel()); // Inicializa el ViewModel
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TInputViewModel inputViewModel)
        {
            if (!ModelState.IsValid)
                return View(inputViewModel);

            try
            {
                var entity = inputViewModel.Export();
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                await _logger.LogInformation($"Se creó {nameof(entity)}", nameof(this.ControllerContext), nameof(Create), JsonSerializer.Serialize(inputViewModel));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                await _logger.LogError("Error al crear entidad", ex, ex.Message, nameof(Create),"");
                ModelState.AddModelError("", _localizer["ErrorCreationMessage"]);
                return View(inputViewModel);
            }
            
        }

        public virtual async Task<IActionResult> Details(TKey key)
        {
            var entity = await _repository.GetByIdAsync(key);

            if (entity == null)
                return NoContent();

            var viewModel = new TDisplayViewModel();
            viewModel.Import(entity);

            return View(viewModel);
        }

        public virtual async Task<IActionResult> Edit(TKey key)
        {
            var entity = await _repository.GetByIdAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            var viewModel = new TInputViewModel();
            viewModel.Import(entity);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public virtual async Task<IActionResult> Edit(TInputViewModel inputModel)
        {
            if (!ModelState.IsValid)
                return View(inputModel);

            try
            {

                var entity = await _repository.GetByIdAsync(inputModel.Id);
                inputModel.Merge(entity);
                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation($"Se Editó {nameof(TEntity)}", ControllerContext.ToString(), nameof(Edit), JsonSerializer.Serialize(inputModel));

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                await _logger.LogError("Error al actualizar entidad", ex, ex.Message, nameof(Edit),"");
                ModelState.AddModelError("", _localizer["ErrorUpdateMessage"]);
                return View(inputModel);
            }

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public virtual async Task<IActionResult> EditWithKeyAndModel(TKey key, TInputViewModel inputModel)
        {
            if (!ModelState.IsValid)
                return View(inputModel);

            try
            {

                var entity = await _repository.GetByIdAsync(key);
                inputModel.Import(entity);
                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation($"Se Editó {nameof(TEntity)}", ControllerContext.ToString(), nameof(EditWithKeyAndModel), JsonSerializer.Serialize(inputModel));

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                await _logger.LogError("Error al actualizar entidad");
                ModelState.AddModelError("", _localizer["ErrorUpdateMessage"]);
                return View(inputModel);
            }


        }

        public virtual async Task<IActionResult> Delete(TKey key)
        {
            var entity = await _repository.GetByIdAsync(key);
            if (entity == null)
                return NotFound();

            var viewModel = new TDisplayViewModel();
            viewModel.Import(entity);
            return View(viewModel);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(TDisplayViewModel displayModel)
        {
            await _repository.DeleteAsync(displayModel.Id);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Elimando en el  {nameof(TDisplayViewModel)}", ControllerContext.ToString(), nameof(Delete), JsonSerializer.Serialize(displayModel));

            return RedirectToAction(nameof(Index));
        }

        protected virtual async Task<PaginatedList<TDisplayViewModel>> GetPaginatedData(
        IQueryable<TEntity> source,
        int pageNumber = 1,
        int pageSize = 10)
        {

            // Mantener los parámetros de búsqueda en la paginación
            var routeValues = ViewBag.RouteValues as RouteValueDictionary ?? new RouteValueDictionary();

            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModels = items.Select(item =>
            {
                var vm = Activator.CreateInstance<TDisplayViewModel>();
                vm.Import(item);
                return vm;
            });

            return new PaginatedList<TDisplayViewModel>(viewModels, count, pageNumber, pageSize, routeValues);
        }
    }
}

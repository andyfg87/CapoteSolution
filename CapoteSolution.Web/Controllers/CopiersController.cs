using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.Models.ViewModels;
using CapoteSolution.Web.Paginations;
using CapoteSolution.Web.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CapoteSolution.Web.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Technician)}")]
    public class CopiersController : AbstractEntityManagementController<Copier, string, CopierInputVM, CopierDisplayVM>
    {
        private readonly IEntityRepository<MachineModel, Guid> _machineModelRepo;
        private readonly IEntityRepository<Customer, Guid> _customerRepo;
        private readonly IEntityRepository<Brand, Guid> _brandRepo;
        private readonly IEntityRepository<Service, Guid> _serviceRepo;
        private readonly IAppLogger _logger;

        public CopiersController(
            IEntityRepository<Copier, string> repository,
            IEntityRepository<MachineModel, Guid> machineModelRepo,
            IEntityRepository<Brand, Guid> brandRepo,
            IEntityRepository<Customer, Guid> customerRepo,
            IEntityRepository<Service, Guid> serviceRepo,
            IStringLocalizer<CopiersController> localizer,
            IAppLogger logger)
            : base(repository, localizer, logger)
        {
            _machineModelRepo = machineModelRepo;
            _customerRepo = customerRepo;
            _brandRepo = brandRepo;
            _serviceRepo = serviceRepo;
            _logger = logger;
        }

        public override async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            // Obtener parámetros de filtro del query string
            string searchId = HttpContext.Request.Query["searchId"].ToString();
            string searchCustomer = HttpContext.Request.Query["searchCustomer"].ToString();
            string invoiceDayStr = HttpContext.Request.Query["InvoiceDay"].ToString();
            string searchModel = HttpContext.Request.Query["searchModel"].ToString();

            ViewBag.InvoiceDays = GetDays();

            var query = await _repository.GetAllWithNestedInclude(
        nameof(MachineModel),
        nameof(Customer),
        $"{nameof(MachineModel)}.{nameof(Toner)}",
        $"{nameof(MachineModel)}.{nameof(Brand)}",
        $"Services.{nameof(ServiceReason)}");

            int invoiceDay = 0;
            int.TryParse(invoiceDayStr, out invoiceDay);

            // Aplicar filtros
            if (!string.IsNullOrEmpty(searchId))
            {
                query = query.Where(c => c.Id.Contains(searchId));
            }

            if (invoiceDay > 0)
            {
                query = query.Where(c => c.InvoiceDay == invoiceDay);
            }

            if (!string.IsNullOrEmpty(searchCustomer))
            {
                query = query.Where(c => c.Customer.CustomerName.Contains(searchCustomer));
            }

            if (!string.IsNullOrEmpty(searchModel))
            {
                query = query.Where(c => c.MachineModel.Name.Contains(searchModel));
            }

            // Crear diccionario para mantener los filtros
            var routeValues = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(searchId)) routeValues.Add("searchId", searchId);
            if (invoiceDay > 0) routeValues.Add("InvoiceDay", invoiceDay.ToString());
            if (!string.IsNullOrEmpty(searchCustomer)) routeValues.Add("searchCustomer", searchCustomer);
            if (!string.IsNullOrEmpty(searchModel)) routeValues.Add("searchModel", searchModel);

            routeValues.Add("sortBy", sortBy);
            routeValues.Add("sortOrder", sortOrder);

            ViewBag.RouteValues = routeValues;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder;

            if (sortBy.ToLower() != "vidatoner")
            {
                query = sortBy.ToLower() switch
                {
                    "id" => sortOrder == "asc" ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id),
                    "modelo" => sortOrder == "asc" ? query.OrderBy(c => c.MachineModel.Name) : query.OrderByDescending(c => c.MachineModel.Name),
                    "cliente" => sortOrder == "asc" ? query.OrderBy(c => c.Customer.CustomerName) : query.OrderByDescending(c => c.Customer.CustomerName),
                    "dia" => sortOrder == "asc" ? query.OrderBy(c => c.InvoiceDay) : query.OrderByDescending(c => c.InvoiceDay),
                    "ultimoservicio" => sortOrder == "asc" ? query.OrderBy(c => c.Services.Max(s => s.Date)) : query.OrderByDescending(c => c.Services.Max(s => s.Date)),
                    "precio" => sortOrder == "asc" ? query.OrderBy(c => c.MonthlyPrice) : query.OrderByDescending(c => c.MonthlyPrice),
                    "comentario" => sortOrder == "asc" ? query.OrderBy(c => c.Comments) : query.OrderByDescending(c => c.Comments),

                };

                var paginatedData = await GetPaginatedData(query, pageNumber, pageSize);
                return View(paginatedData);
            }
            else
            {
                // Para vidatoner: solución especial
                var count = await query.CountAsync();
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Convertir a ViewModel y ordenar por TonerLife
                var viewModels = items.Select(item =>
                {
                    var vm = new CopierDisplayVM();
                    vm.Import(item);
                    return vm;
                });

                viewModels = sortOrder == "asc" ?
                    viewModels.OrderBy(vm => vm.TonerLife()) :
                    viewModels.OrderByDescending(vm => vm.TonerLife());

                // Crear PaginatedList manualmente
                var paginatedData = new PaginatedList<CopierDisplayVM>(
                    viewModels,
                    count,
                    pageNumber,
                    pageSize,
                    ViewBag.RouteValues as RouteValueDictionary
                );

                return View(paginatedData);
            }            

           

            /*var paginatedData = await GetPaginatedData(query, pageNumber, pageSize);
            return View(paginatedData);*/
        }        

        public override async Task<IActionResult> Create()
        {
            var model = new CopierInputVM
            {
                AvailableBrands = await GetBrands(),
                AvailableMachineModels = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "Seleccione una marca primero" }
                }, "Value", "Text"),
                AvailableCustomers = await GetCustomers()
            };
            return View(model);
        }

        public override async Task<IActionResult> Create(CopierInputVM inputViewModel)
        {
            if (!ModelState.IsValid)
                return View(inputViewModel);

            try
            {
                var entity = inputViewModel.Export();
                entity.Id = inputViewModel.Id;
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                ViewData["ShowSuccessDialog"] = true;
                ViewData["NewCopierId"] = entity.Id;

                // Recargamos los datos necesarios para la vista
                inputViewModel.AvailableBrands = await GetBrands();
                inputViewModel.AvailableCustomers = await GetCustomers();
                inputViewModel.AvailableMachineModels = new SelectList(new List<SelectListItem>());

                _logger.LogInformation($"Se Creó {nameof(Copier)}", nameof(CopiersController), nameof(Create), JsonSerializer.Serialize(inputViewModel));

                return View(inputViewModel);

                //return RedirectToAction(nameof(Index));               
            }
            catch (Exception ex)
            {

                _logger.LogError("Error al crear entidad", ex, ex.Message, nameof(Create), "");
                ModelState.AddModelError("", _localizer["ErrorCreationMessage"]);
                return View(inputViewModel);
            }
        }

        public override async Task<IActionResult> Edit(string key)
        {
            var entity = await _repository.GetAllWithNestedInclude(
                nameof(MachineModel),
                nameof(Customer),
                nameof(MachineModel) + "." + nameof(Toner),
                nameof(MachineModel) + "." + nameof(Brand))
                .Result.FirstAsync(c => c.Id == key);

            if (entity != null)
            {

                var copierInputVM = new CopierInputVM();
                copierInputVM.Import(entity);
                copierInputVM.AvailableMachineModels = await GetMachineModels();
                copierInputVM.AvailableBrands = await GetBrands();
                copierInputVM.AvailableCustomers = await GetCustomers();
                return View(copierInputVM);
            }

            return NotFound();

        }

        [HttpPost]
        public override Task<IActionResult> EditWithKeyAndModel(string id, CopierInputVM inputModel)
        {
            return base.Edit(inputModel);
        }

        [HttpGet]
        public async Task<IActionResult> ContractDetails(string id)
        {
            var contract = await _repository.GetAll().Result
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

       
        public override async Task<IActionResult> Details(string key)
        {
            var entity = await _repository.GetAllWithNestedInclude(nameof(MachineModel),
                nameof(Customer),    //Include           
                nameof(MachineModel) + "." + nameof(Toner),// ThenInclude
                nameof(MachineModel) + "." + nameof(Brand),// ThenInclude
                "Services", //Incluye la lista Services => ThenInclude
                "Services." + nameof(ServiceReason),// ThenInclude
                "Services." + "Technician").Result.FirstAsync(c => c.Id == key);



            ViewBag.ServiceMonthlyCounter = entity.Services.Where(s => s.ServiceReason.Name == ServiceReason.Reasons.MonthlyCounter).OrderBy(s => s.Date);
            ViewBag.ServiceTonerChange = entity.Services.Where(s => s.ServiceReason.Name == ServiceReason.Reasons.TonerChange).OrderBy(s => s.Date);
            ViewBag.ServiceMaintenance = entity.Services.Where(s => s.ServiceReason.Name == ServiceReason.Reasons.Maintenance).OrderBy(s => s.Date);

            var viewModel = new CopierDisplayVM();
            viewModel.Import(entity);

            return View(viewModel);
        }

            
        public async Task<IActionResult> DetailsCopierByServicePagination(string key,
        int tonerPage = 1,
        int maintenancePage = 1,
        int monthlyPage = 1,
        int pageSize = 5)
        {
            var entity = await _repository.GetAllWithNestedInclude(nameof(MachineModel),
                 nameof(Customer),    //Include           
                 $"{nameof(MachineModel)}.{nameof(Toner)}",// ThenInclude
                 $"{nameof(MachineModel)}.{nameof(Brand)}",// ThenInclude
                 $"Services", //Incluye la lista Services => ThenInclude
                 $"Services.{nameof(ServiceReason)}",// ThenInclude
                 $"Services.Technician").Result.FirstAsync(c => c.Id == key);

            // Servicios ordenados para cálculo de diferencias
            var allMonthlyServices = entity.Services
                .Where(s => s.ServiceReason.Name == ServiceReason.Reasons.MonthlyCounter)
                .OrderBy(s => s.Date)
                .ToList();

            // Paginación
            ViewBag.ServiceMonthlyCounter = PaginatedList<Service>.Create(
                allMonthlyServices,
                monthlyPage,
                pageSize);

            ViewBag.AllMonthlyServices = allMonthlyServices; // Para cálculo de diferencias

            // Resto de tus servicios paginados (sin necesidad de lista completa)
            ViewBag.ServiceTonerChange = PaginatedList<Service>.Create(
                entity.Services
                    .Where(s => s.ServiceReason.Name == ServiceReason.Reasons.TonerChange)
                    .OrderBy(s => s.Date),
                tonerPage,
                pageSize);

            ViewBag.ServiceMaintenance = PaginatedList<Service>.Create(
                entity.Services
                    .Where(s => s.ServiceReason.Name == ServiceReason.Reasons.Maintenance)
                    .OrderBy(s => s.Date),
                maintenancePage,
                pageSize);

            var viewModel = new CopierDisplayVM();
            viewModel.Import(entity);

            return View(viewModel);
        }

        public override async Task<IActionResult> Delete(string key)
        {
            var result = await _repository.GetAllWithNestedInclude(nameof(MachineModel),
                 nameof(Customer),    //Include           
                 nameof(MachineModel) + "." + nameof(Toner),// ThenInclude
                 nameof(MachineModel) + "." + nameof(Brand),// ThenInclude
                 "Services", //Incluye la lista Services => ThenInclude
                 "Services." + nameof(ServiceReason),// ThenInclude
                 "Services." + "Technician");
            var entity = result.FirstAsync(c => c.Id == key).Result;

            if (entity == null)
                return NotFound();

            var viewModel = new CopierDisplayVM();
            viewModel.Import(entity);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMachineModelsByBrand(Guid brandId, Guid machineModelId)
        {
            if (brandId == Guid.Empty)
            {
                return Json(new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione una marca válida" }
        });
            }

            var machineModels = await _machineModelRepo.GetAll().Result
                .Include(mm => mm.Brand)
                .Where(mm => mm.BrandId == brandId)
                .Select(mm => new SelectListItem
                {
                    Value = mm.Id.ToString(),
                    Text = $"{mm.Brand.Name} - {mm.Name}",
                    Selected = mm.Id == machineModelId
                })
                .ToListAsync();

            if (!machineModels.Any())
            {
                machineModels = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "No hay modelos para esta marca" }
        };
            }

            return Json(machineModels);
        }

        public async Task<IActionResult> GenerateReport(string key, string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime startDateParsed))
                return BadRequest("Fecha de inicio inválida");

            if (!DateTime.TryParse(endDate, out DateTime endDateParsed))
                return BadRequest("Fecha de fin inválida");

            // Ajustar endDate para incluir todo el día
            endDateParsed = endDateParsed.Date.AddDays(1).AddSeconds(-1);

            var entity = await _repository.GetAllWithNestedInclude(nameof(MachineModel),
                  nameof(Customer),    //Include           
                  $"{nameof(MachineModel)}.{nameof(Toner)}",// ThenInclude
                  $"{nameof(MachineModel)}.{nameof(Brand)}",// ThenInclude
                  $"Services", //Incluye la lista Services => ThenInclude
                  $"Services.{nameof(ServiceReason)}",// ThenInclude
                  $"Services.Technician").Result.FirstAsync(c => c.Id == key);

            // Filtrar solo servicios de contador mensual y ordenar por fecha
            var servicesOrderByDate = entity.Services
                .Where(s => s.Date >= startDateParsed &&
                           s.Date <= endDateParsed &&
                           s.ServiceReason.Name == ServiceReason.Reasons.MonthlyCounter)
                .OrderBy(s => s.Date)
                .ToList();

            // Crear la lista de reportes con información de diferencia
            var copiersReports = new List<CopierReportItem>();

            for (int i = 0; i < servicesOrderByDate.Count; i++)
            {
                var currentService = servicesOrderByDate[i];
                var previousService = i > 0 ? servicesOrderByDate[i - 1] : null;

                // Calcular diferencias
                int blackDifference = previousService != null ?
                    currentService.BlackCounter - previousService.BlackCounter : 0;

                int colorDifference = previousService != null ?
                    currentService.ColorCounter - previousService.ColorCounter : 0;

                // Calcular extras
                int extraBlack = Math.Max(0, blackDifference - (int)entity.PlanBW);
                int extraColor = Math.Max(0, colorDifference - (int)entity.PlanColor);

                copiersReports.Add(new CopierReportItem
                {
                    PrinterId = key,
                    Month = currentService.Date.ToString("MMM yyyy"),
                    Date = currentService.Date,
                    BlackCounter = currentService.BlackCounter,
                    ColorCounter = currentService.ColorCounter,
                    PlanBw = (int)entity.PlanBW,
                    PlanColor = (int)entity.PlanColor,
                    BlackDifference = blackDifference,
                    ColorDifference = colorDifference,
                    ExtraBlack = extraBlack,
                    ExtraColor = extraColor,
                    TotalBlack = currentService.BlackCounter,
                    TotalColor = currentService.ColorCounter,
                    TotalCopies = currentService.BlackCounter + currentService.ColorCounter,
                    IsMonthlyCounter = true,
                    PreviousBlack = previousService?.BlackCounter,
                    PreviousColor = previousService?.ColorCounter
                });
            }

            var displayVM = new CopierDisplayVM();
            displayVM.Import(entity);

            var pdfBytes = new CopierReportGenerator(copiersReports, key, displayVM).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"{key}-ReporteMensual-{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        public async Task<SelectList> GetBrands()
        {
            var brands = await _brandRepo.GetAll().Result
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToListAsync();

            var selectListBrands = new SelectList(brands, "Value", "Text");
            return selectListBrands;
        }

        private async Task<SelectList> GetMachineModels(string IdBrand = "")
        {
            var machineModels = await _machineModelRepo.GetAll().Result
                .Include(mm => mm.Brand)
                .Where(mm => mm.BrandId.ToString() == IdBrand)
                .Select(mm => new
                {
                    mm.Id,
                    DisplayText = $"{mm.Brand.Name} - {mm.Name}"
                })
                .ToListAsync();

            return new SelectList(machineModels, "Id", "DisplayText");
        }

        private async Task<SelectList> GetCustomers()
        {
            var customers = await _customerRepo.GetAll().Result
                .Select(c => new { c.Id, DisplayText = c.CustomerName })
                .ToListAsync();

            return new SelectList(customers, "Id", "DisplayText");
        }

        private async Task<PaginatedList<Service>> PaginateServices(IEnumerable<Service> services, int pageNumber, int pageSize)
        {
            return await PaginatedList<Service>.CreateAsync(services.AsQueryable(), pageNumber, pageSize);
        }

        private SelectList GetDays()
        {
            var days = _repository.GetAll().Result
                .Select(c =>c.InvoiceDay).Distinct()
                .Select(c => new { Value = c, Text = c });

            return new SelectList(days, "Value", "Text");
        }
    }
}

using CapoteSolution.Models.Entities;
using CapoteSolution.Web.Interface;
using CapoteSolution.Web.MenuInfo;
using CapoteSolution.Web.Models;
using CapoteSolution.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace CapoteSolution.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEntityRepository<Brand, Guid> _brandRepository;
        private readonly IEntityRepository<Toner, Guid> _tonerRepository;
        private readonly IEntityRepository<Service, Guid> _serviceRepository;
        private readonly IEntityRepository<MachineModel, Guid> _machineModelRepository;
        private readonly IEntityRepository<Copier, string> _copierRepository;
        private readonly ContractRepository _contractRepository;

        public HomeController(ILogger<HomeController> logger, IEntityRepository<Toner, Guid> tonerRepository, IEntityRepository<Service, Guid> serviceRepository, IEntityRepository<MachineModel, Guid> machineModelRepository, IEntityRepository<Copier, string> copierRepository, ContractRepository contractRepository)
        {
            _logger = logger;
            _tonerRepository = tonerRepository;
            _serviceRepository = serviceRepository;
            _machineModelRepository = machineModelRepository;
            _copierRepository = copierRepository;
            _contractRepository = contractRepository;
        }

        public IActionResult Index()
        {
            var counts = new  MenuCounts
            {
                ActiveContracts = _contractRepository.ActiveContracts()
            };

            ViewData["MenuCounts"] = counts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }    
}

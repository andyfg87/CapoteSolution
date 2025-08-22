using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Paginations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapoteSolution.Web.Controllers
{
    public class LogsController : Controller
    {
        private readonly IAppLogger _logger;

        public LogsController(IAppLogger logger)
        {
            _logger = logger;
        }
        // GET: LogsController
        public async Task<ActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var query = _logger.ApplicationLogs().Result;

            var paginatedData = new PaginatedList<ApplicationLog>(query, query.Count(), pageNumber, pageSize);

            return View(paginatedData);
        }

        [HttpGet]
        public async Task<IActionResult> Details( int key) 
        {
            var result = _logger.ApplicationLogs().Result;

            var entity = await result.FirstOrDefaultAsync(l => l.Id == key);

            if (entity == null) 
            {
                return NotFound();
            }

            return View(entity);
        }
    }
}

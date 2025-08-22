using CapoteSolution.Models.EF;
using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Extensions;
using System.Text.Json;

namespace CapoteSolution.Web.Repositories
{
    public class DatabaseLogger : IAppLogger
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _httpContext;

        public DatabaseLogger(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;           
        }

        public async Task LogInformation(string message, string logger = null, string action = null, object parameters = null)
        {
            await LogAsync("INFO", message, null, logger, action, parameters);
        }

        public async Task LogError(string message, Exception ex = null, string logger = null, string action = null, object parameters = null)
        {
            await LogAsync("ERROR", message, ex?.ToString(), logger, action, parameters);
        }

        private async Task LogAsync(string level, string message, string exception, string logger, string action, object parameters)
        {
            string formattedParameters = "null";

            if (parameters != null) 
            {               

                formattedParameters = parameters.ToSanitizedJson();
            }
            
            var log = new ApplicationLog
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message,
                Logger = logger,
                Exception = exception,
                Action = action,
                Parameters = formattedParameters,
                User = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System"
                
            };

            _context.ApplicationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public Task LogWarning(string message, string logger = null, string action = null, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public Task LogPerformance(string action, long durationMs, string logger = null, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<ApplicationLog>> ApplicationLogs()
        {
            return  _context.ApplicationLogs;
        }

       
    }
}

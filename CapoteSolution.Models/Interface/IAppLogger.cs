using CapoteSolution.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Interface
{
    public interface IAppLogger
    {
        Task LogInformation(string message, string logger = null, string action = null, object parameters = null);
        Task LogWarning(string message, string logger = null, string action = null, object parameters = null);
        Task LogError(string message, Exception ex = null, string logger = null, string action = null, object parameters = null);
        Task LogPerformance(string action, long durationMs, string logger = null, object parameters = null);
        Task<IQueryable<ApplicationLog>> ApplicationLogs();
    }
}

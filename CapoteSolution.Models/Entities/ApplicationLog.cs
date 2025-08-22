using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Entities
{
    public class ApplicationLog
    {  
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } // "INFO", "WARNING", "ERROR", "DEBUG"
        public string Message { get; set; }
        public string Logger { get; set; } // Nombre del controlador/service
        public string? Exception { get; set; }
        public string User { get; set; }
        public string Action { get; set; } // "GenerateReport", "Create", etc.
        public string? Parameters { get; set; } // JSON de parámetros
        public long? DurationMs { get; set; } // Duración de la operación
        

        public string SanitizeParameters(object parameters)
        {
            if (parameters == null) return "null";

            try
            {
                // Convertir a JSON
                var json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                // Remover datos sensibles
                var sensitiveFields = new[] { "Password", "PasswordHash", "Token", "Secret", "CreditCard" };
                foreach (var field in sensitiveFields)
                {
                    json = System.Text.RegularExpressions.Regex.Replace(
                        json,
                        $@"""{field}""\s*:\s*""[^""]*""",
                        $@"""{field}"": ""***REDACTED***""",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );
                }

                return json;
            }
            catch
            {
                return parameters.ToString();
            }
        }
    }
}

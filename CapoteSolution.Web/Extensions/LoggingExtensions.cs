using System.Text.Json;
using System.Text.RegularExpressions;

namespace CapoteSolution.Web.Extensions
{
    public static class LoggingExtensions
    {
        private static readonly string[] _sensitiveFields =
        {
            "Password", "PasswordHash", "Token", "Secret",
            "CreditCard", "SecurityCode", "CVV", "ApiKey",
            "ConnectionString", "JwtToken", "AccessToken"
        };

        public static string ToSanitizedJson(this object obj)
        {
            if (obj == null) return "null";

            try
            {
                // Serializar a JSON sin escape de caracteres
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(obj, options);

                // Sanitizar campos sensibles
                return SanitizeSensitiveData(json);
            }
            catch (Exception ex)
            {
                return $"{{\"error\": \"Failed to serialize: {ex.Message}\"}}";
            }
        }

        private static string SanitizeSensitiveData(string json)
        {
            foreach (var field in _sensitiveFields)
            {
                // Patrón para encontrar: "field": "valor"
                var pattern = $@"""{field}""\s*:\s*""[^""]*""";
                json = Regex.Replace(json, pattern, $@"""{field}"": ""***REDACTED***""", RegexOptions.IgnoreCase);

                // Patrón para encontrar: "field": null
                pattern = $@"""{field}""\s*:\s*null";
                json = Regex.Replace(json, pattern, $@"""{field}"": ""***REDACTED***""", RegexOptions.IgnoreCase);

                // Patrón para encontrar: 'field': 'valor' (comillas simples)
                pattern = $@"'{field}'\s*:\s*'[^']*'";
                json = Regex.Replace(json, pattern, $@"'{field}': '***REDACTED***'", RegexOptions.IgnoreCase);
            }

            return json;
        }

        public static object ToLogSafeObject(this object obj)
        {
            if (obj == null) return null;

            // Si es una string, devolverla directamente
            if (obj is string str) return str;

            // Si es un tipo simple, devolverlo
            if (obj.GetType().IsPrimitive || obj is decimal || obj is DateTime)
                return obj;

            // Para objetos complejos, crear un DTO seguro
            var safeProperties = new Dictionary<string, object>();
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.CanRead)
                {
                    var value = prop.GetValue(obj);
                    var propName = JsonNamingPolicy.CamelCase.ConvertName(prop.Name);

                    // Sanitizar campos sensibles
                    if (_sensitiveFields.Any(f =>
                        prop.Name.Contains(f, StringComparison.OrdinalIgnoreCase)))
                    {
                        safeProperties[propName] = "***REDACTED***";
                    }
                    else
                    {
                        safeProperties[propName] = value;
                    }
                }
            }

            return safeProperties;
        }
    }
}

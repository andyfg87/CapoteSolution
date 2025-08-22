using System.Text.Json;

namespace CapoteSolution.Web.Extensions
{
    public static class JsonExtensions
    {
        public static string UnescapeJson(this string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return jsonString;

            try
            {
                // Primero, desescapar las comillas dobles
                string unescaped = jsonString.Replace("\\\"", "\"");

                // Remover escapes de Unicode
                unescaped = System.Text.RegularExpressions.Regex.Unescape(unescaped);

                // Si empieza y termina con comillas, removerlas
                if (unescaped.StartsWith("\"") && unescaped.EndsWith("\""))
                {
                    unescaped = unescaped.Substring(1, unescaped.Length - 2);
                }

                return unescaped;
            }
            catch
            {
                return jsonString;
            }
        }

        public static string FormatJson(this string jsonString)
        {
            try
            {
                var unescaped = jsonString.UnescapeJson();

                // Intentar parsear y formatear como JSON bonito
                var jsonDoc = JsonDocument.Parse(unescaped);
                return JsonSerializer.Serialize(jsonDoc.RootElement, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch
            {
                // Si no es JSON válido, devolver el texto original
                return jsonString.UnescapeJson();
            }
        }
    }
}

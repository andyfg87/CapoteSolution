using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CapoteSolution.Web.Reports
{
    public class CopierReportGenerator
    {
        private readonly List<CopierReportItem> _data;
        private string _copierId;

        public CopierReportGenerator(List<CopierReportItem> data, string copierId)
        {
            _data = data;
            _copierId = copierId;
        }

        public byte[] GeneratePdf()
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text($"Reporte de Copias de Impresora {_copierId}")
                        .Bold().FontSize(16);

                    page.Content()
                        .Column(col =>
                        {
                            // Tabla de datos
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {                                   
                                    columns.RelativeColumn(); // Mes
                                    columns.RelativeColumn(); // PlanBW
                                    columns.RelativeColumn(); // B/N
                                    columns.RelativeColumn(); // PlanColor
                                    columns.RelativeColumn(); // Color
                                });

                                // Encabezados
                                table.Header(header =>
                                {                                    
                                    header.Cell().Text("Mes").Bold();
                                    header.Cell().Text("Plan B/N").Bold();
                                    header.Cell().Text("Copias B/N").Bold();
                                    header.Cell().Text("plan Color").Bold();
                                    header.Cell().Text("Copias Color").Bold();
                                });

                                // Datos
                                foreach (var item in _data)
                                {                                    
                                    table.Cell().Text(item.Month);
                                    table.Cell().Text(item.PlanBW.ToString("N0"));
                                    table.Cell().Text(item.BlackCopies.ToString("N0"));
                                    table.Cell().Text(item.PlanColor.ToString("N0"));
                                    table.Cell().Text(item.ColorCopies.ToString("N0"));
                                }
                            });

                            // Gráfico de barras (apiladas)
                            col.Item().PaddingTop(20).Row(row =>
                            {
                                row.RelativeItem().Component(new CopierBarChartComponent(_data));
                            });
                        });
                });
            }).GeneratePdf();
        }
    }
}

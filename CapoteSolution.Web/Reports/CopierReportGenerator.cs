using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CapoteSolution.Web.Reports
{
    public class CopierReportGenerator
    {
        private readonly List<CopierReportItem> _data;
        private string _copierId;
        private string _customerName;

        public CopierReportGenerator(List<CopierReportItem> data, string copierId, string customerName)
        {
            _data = data;
            _copierId = copierId;
            _customerName = customerName;
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
                        .Text($"Reporte de Copias - Impresora {_copierId} {_customerName}")
                        .Bold().FontSize(16);

                    page.Content()
                        .Column(col =>
                        {
                            // Tabla de datos principal
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Mes
                                    columns.RelativeColumn(); // BlackCounter
                                    columns.RelativeColumn(); // BlackDifference
                                    columns.RelativeColumn(); // ExtraBlack
                                    columns.RelativeColumn(); // ColorCounter
                                    columns.RelativeColumn(); // ColorDifference
                                    columns.RelativeColumn(); // ExtraColor
                                    columns.RelativeColumn(); // TotalCopies
                                });

                                // Encabezados
                                table.Header(header =>
                                {
                                    header.Cell().Text("Mes").Bold();
                                    header.Cell().Text("Contador B/N").Bold();
                                    header.Cell().Text("Diferencia B/N").Bold();
                                    header.Cell().Text("Extra B/N").Bold();
                                    header.Cell().Text("Contador Color").Bold();
                                    header.Cell().Text("Diferencia Color").Bold();
                                    header.Cell().Text("Extra Color").Bold();
                                    header.Cell().Text("Total").Bold();
                                });

                                // Datos
                                foreach (var item in _data)
                                {
                                    table.Cell().Text(item.Month).FontSize(10);
                                    table.Cell().Text(item.BlackCounter.ToString("N0")).FontSize(10);
                                    table.Cell().Text(item.BlackDifference.ToString("N0")).FontSize(10);
                                    table.Cell().Text(item.ExtraBlack.ToString("N0")).FontSize(10);
                                    table.Cell().Text(item.ColorCounter.ToString("N0")).FontSize(10);
                                    table.Cell().Text(item.ColorDifference.ToString("N0")).FontSize(10);
                                    table.Cell().Text(item.ExtraColor.ToString("N0")).FontSize(10);
                                    table.Cell().Text(item.TotalCopies.ToString("N0")).FontSize(10).SemiBold();
                                }
                            });

                            // Resumen total al final
                            col.Item().PaddingTop(15).Table(summaryTable =>
                            {
                                summaryTable.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                summaryTable.Cell().Text("Total Copias B/N:").SemiBold();
                                summaryTable.Cell().Text(_data.Sum(x => x.BlackCounter).ToString("N0")).SemiBold();

                                summaryTable.Cell().Text("Total Copias Color:").SemiBold();
                                summaryTable.Cell().Text(_data.Sum(x => x.ColorCounter).ToString("N0")).SemiBold();

                                summaryTable.Cell().Text("Total General:").SemiBold();
                                summaryTable.Cell().Text(_data.Sum(x => x.TotalCopies).ToString("N0")).SemiBold();

                                summaryTable.Cell().Text("Total Extra B/N:").SemiBold();
                                summaryTable.Cell().Text(_data.Sum(x => x.ExtraBlack).ToString("N0")).SemiBold();

                                summaryTable.Cell().Text("Total Extra Color:").SemiBold();
                                summaryTable.Cell().Text(_data.Sum(x => x.ExtraColor).ToString("N0")).SemiBold();
                            });
                        });
                });
            }).GeneratePdf();
        }
    }
}
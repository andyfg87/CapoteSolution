using CapoteSolution.Web.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CapoteSolution.Web.Reports
{
    public class CopierReportGenerator
    {
        private readonly List<CopierReportItem> _data;
        private string _copierId;
        private CopierDisplayVM _copier;

        public CopierReportGenerator(List<CopierReportItem> data, string copierId, CopierDisplayVM copier)
        {
            _data = data;
            _copierId = copierId;
            _copier = copier;
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
                        .Text($"Reporte de Copias - Impresora {_copierId} {_copier.CustomerName}")
                        .Bold().FontSize(16);

                    page.Content()
                        .Column(col =>
                        {
                            // Tarjetas de información (similar al HTML)
                            col.Item().Row(row =>
                            {
                                // Primera tarjeta: Información de la impresora
                                row.RelativeItem().PaddingRight(5).Background(Colors.Blue.Lighten4).Padding(10).Column(card =>
                                {
                                    card.Item().PaddingBottom(5).Text("Información Impresora").Bold().FontColor(Colors.Blue.Darken3);

                                    card.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().Text("Estado:").SemiBold();
                                        table.Cell().Text(_copier.Status.ToString()).FontColor(
                                            _copier.Status.ToString() == "Active" ? Colors.Green.Darken3 : Colors.Orange.Darken3);

                                        table.Cell().Text("No Serie:").SemiBold();
                                        table.Cell().Text(_copier.SerialNumber);

                                        table.Cell().Text("Dirección IP:").SemiBold();
                                        table.Cell().Text(_copier.IPAddress);

                                        table.Cell().Text("Correo:").SemiBold();
                                        table.Cell().Text(_copier.MachineEmail);

                                        table.Cell().Text("Modelo:").SemiBold();
                                        table.Cell().Text(_copier.MachineModelInfo);
                                    });
                                });

                                // Segunda tarjeta: Información del contrato
                                row.RelativeItem().PaddingHorizontal(5).Background(Colors.Blue.Lighten4).Padding(10).Column(card =>
                                {
                                    card.Item().PaddingBottom(5).Text("Contrato").Bold().FontColor(Colors.Blue.Darken3);

                                    card.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().Text("Plan B/N:").SemiBold();
                                        table.Cell().Text($"{_copier.PlanBW?.ToString("N0")} copias/mes");

                                        table.Cell().Text("Precio Extra B/N:").SemiBold();
                                        table.Cell().Text(_copier.ExtraBW?.ToString("C") + " por copia");

                                        table.Cell().Text("Plan Color:").SemiBold();
                                        table.Cell().Text($"{_copier.PlanColor?.ToString("N0")} copias/mes");

                                        table.Cell().Text("Precio Extra Color:").SemiBold();
                                        table.Cell().Text(_copier.ExtraColor?.ToString("C") + " por copia");

                                        table.Cell().Text("Precio Mensual:").SemiBold();
                                        table.Cell().Text(_copier.MonthlyPrice?.ToString("C"));
                                    });
                                });

                                // Tercera tarjeta: Vida del Toner
                                row.RelativeItem().PaddingLeft(5).Background(Colors.Blue.Lighten4).Padding(10).Column(card =>
                                {
                                    card.Item().PaddingBottom(5).Text("Vida del Toner").Bold().FontColor(Colors.Blue.Darken3);

                                    card.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().Text("Toners Cambiados:").SemiBold();
                                        table.Cell().Text(_copier.QtyOfHightestTonerChange?.ToString());

                                        table.Cell().Text("Total Yield:").SemiBold();
                                        table.Cell().Text(_copier.TotalYield());

                                        table.Cell().Text("Change In:").SemiBold();
                                        table.Cell().Text(_copier.ChangeIn());

                                        table.Cell().Text("Toner Life:").SemiBold();
                                        table.Cell().Text(_copier.TonerLife());
                                    });
                                });
                            });

                            // Tabla de datos principal
                            col.Item().PaddingTop(15).Table(table =>
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
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Mes").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Contador B/N").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Diferencia B/N").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Extra B/N").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Contador Color").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Diferencia Color").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Extra Color").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total").Bold();
                                });

                                // Datos
                                foreach (var item in _data)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Month).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.BlackCounter.ToString("N0")).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.BlackDifference.ToString("N0")).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.ExtraBlack.ToString("N0")).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.ColorCounter.ToString("N0")).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.ColorDifference.ToString("N0")).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.ExtraColor.ToString("N0")).FontSize(10);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.TotalCopies.ToString("N0")).FontSize(10).SemiBold();
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

                                // Agregar cálculo de costos
                                summaryTable.Cell().Text("Costo Total Extra B/N:").SemiBold();
                                summaryTable.Cell().Text((_data.Sum(x => x.ExtraBlack) * (_copier.ExtraBW ?? 0)).ToString("C")).SemiBold();

                                summaryTable.Cell().Text("Costo Total Extra Color:").SemiBold();
                                summaryTable.Cell().Text((_data.Sum(x => x.ExtraColor) * (_copier.ExtraColor ?? 0)).ToString("C")).SemiBold();

                                summaryTable.Cell().Text("Costo Total:").SemiBold();
                                summaryTable.Cell().Text(((_data.Sum(x => x.ExtraBlack) * (_copier.ExtraBW ?? 0)) +
                                                         (_data.Sum(x => x.ExtraColor) * (_copier.ExtraColor ?? 0)) +
                                                         (_copier.MonthlyPrice ?? 0)).ToString("C")).SemiBold();
                            });
                        });
                });
            }).GeneratePdf();
        }
    }
}
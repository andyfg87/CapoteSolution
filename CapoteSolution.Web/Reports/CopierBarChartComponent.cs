using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CapoteSolution.Web.Reports
{
    public class CopierBarChartComponent : IComponent
    {
        private readonly List<CopierReportItem> _data;

        public CopierBarChartComponent(List<CopierReportItem> data)
        {
            _data = data.GroupBy(x => x.Month)
                .Select(g => new CopierReportItem
                {                    
                    Month = g.Key,
                    BlackCopies = g.Sum(x => x.BlackCopies),
                    ColorCopies = g.Sum(x => x.ColorCopies)
                })
                .OrderBy(x => x.Month)
                .ToList();
        }

        /*public void Compose(IContainer container)
        {
            // Asegura que maxTotal sea al menos 1 para evitar división por cero
            var maxTotal = Math.Max(_data.Max(x => x.BlackCopies + x.ColorCopies), 1);

            container.Column(column =>
            {
                // Leyenda
                column.Item().Row(legendRow =>
                {
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Blue.Darken1);
                    legendRow.AutoItem().Text(" B/N").FontSize(9);
                    legendRow.Spacing(10);
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Red.Darken1);
                    legendRow.AutoItem().Text(" Color").FontSize(9);
                });

                // Barras
                foreach (var item in _data)
                {
                    column.Item().Row(row =>
                    {
                        // Etiqueta
                        row.AutoItem().Width(90)
                           .Text($"{item.Month}").FontSize(10);

                        // Barras apiladas
                        row.RelativeItem().Height(25).Stack(stack =>
                        {
                            // Barra B/N (azul)
                            if (item.BlackCopies > 0)
                            {
                                stack.Item()
                                    .Background(Colors.Blue.Darken1)
                                    .Width(item.BlackCopies / (float)maxTotal * 100)
                                    .Height(20) // Altura fija
                                    .AlignMiddle()
                                    .Text(item.BlackCopies.ToString("N0"),
                                        TextStyle.Default.Size(8).Color(Colors.White));
                            }

                            // Barra Color (rojo)
                            if (item.ColorCopies > 0)
                            {
                                stack.Item()
                                    .Background(Colors.Red.Darken1)
                                    .Width(item.ColorCopies / (float)maxTotal * 100)
                                    .Height(20) // Altura fija
                                    .AlignMiddle()
                                    .Text(item.ColorCopies.ToString("N0"),
                                        TextStyle.Default.Size(8).Color(Colors.White));
                            }
                        });

                        // Total
                        row.AutoItem().Width(40)
                           .Text((item.BlackCopies + item.ColorCopies).ToString("N0"))
                           .FontSize(10);
                    });
                }
            });
        }*/

        /*public void Compose(IContainer container)
        {
            var maxTotal = Math.Max(_data.Max(x => x.BlackCopies + x.ColorCopies), 1);

            container.Column(column =>
            {
                // Leyenda mejorada
                column.Item().Row(legendRow =>
                {
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Blue.Darken1);
                    legendRow.AutoItem().Text(" B/N").FontSize(9);
                    legendRow.Spacing(10);
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Red.Darken1);
                    legendRow.AutoItem().Text(" Color").FontSize(9);
                });

                // Barras horizontales mejoradas
                foreach (var item in _data)
                {
                    var total = item.BlackCopies + item.ColorCopies;

                    column.Item().Row(row =>
                    {
                        // Fecha (formato más corto)
                        row.AutoItem().Width(80)
                           .Text(item.Month.Substring(0, 5)) // Muestra solo "08/11"
                           .FontSize(10);

                        // Barras apiladas con bordes
                        row.RelativeItem().Height(25).Stack(stack =>
                        {
                            // Barra B/N (con borde para mejor definición)
                            stack.Item().Border(1).BorderColor(Colors.Grey.Lighten2)
                                .Background(Colors.Blue.Darken1)
                                .Width(item.BlackCopies / (float)maxTotal * 100)
                                .Height(20)
                                .AlignMiddle()
                                .Text(item.BlackCopies > 0 ? item.BlackCopies.ToString("N0") : "",
                                    TextStyle.Default.Size(8).Color(Colors.White));

                            // Barra Color (solo si hay valores)
                            if (item.ColorCopies > 0)
                            {
                                stack.Item().Border(1).BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Red.Darken1)
                                    .Width(item.ColorCopies / (float)maxTotal * 100)
                                    .Height(20)
                                    .AlignMiddle()
                                    .Text(item.ColorCopies.ToString("N0"),
                                        TextStyle.Default.Size(8).Color(Colors.White));
                            }
                        });

                        // Total con formato consistente
                        row.AutoItem().Width(50)
                           .Text(total.ToString("N0"))
                           .FontSize(10).SemiBold();
                    });
                }
            });
        }*/

        public void Compose(IContainer container)
        {
            var maxTotal = Math.Max(_data.Max(x => x.BlackCopies + x.ColorCopies), 1);

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(90); // Fecha
                    columns.RelativeColumn();   // Barras
                    columns.ConstantColumn(40); // Total
                });

                // Leyenda
                table.Cell().ColumnSpan(3).Row(legendRow =>
                {
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Blue.Darken1);
                    legendRow.AutoItem().Text(" B/N").FontSize(9);
                    legendRow.Spacing(10);
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Red.Darken1);
                    legendRow.AutoItem().Text(" Color").FontSize(9);
                });

                // Datos
                foreach (var item in _data)
                {
                    // Fecha
                    table.Cell().Text(item.Month).FontSize(10);

                    // Barras (solución más estable)
                    table.Cell().Height(25).Background(Colors.Grey.Lighten3).Column(stack =>
                    {
                        if (item.BlackCopies > 0)
                        {
                            stack.Item().Background(Colors.Blue.Darken1)
                                 .Width(item.BlackCopies / (float)maxTotal * 100)
                                 .Height(25);
                        }
                        if (item.ColorCopies > 0)
                        {
                            stack.Item().Background(Colors.Red.Darken1)
                                 .Width(item.ColorCopies / (float)maxTotal * 100)
                                 .Height(25);
                        }
                    });

                    // Total
                    table.Cell().Text((item.BlackCopies + item.ColorCopies).ToString("N0"))
                             .FontSize(10);
                }
            });
        }
    }
}

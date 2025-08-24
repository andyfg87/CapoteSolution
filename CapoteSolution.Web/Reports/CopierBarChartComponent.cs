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
            // Agrupar por mes y sumar los contadores
            _data = data
                .GroupBy(x => x.Month)
                .Select(g => new CopierReportItem
                {
                    Month = g.Key,
                    BlackCounter = g.Sum(x => x.BlackCounter),
                    ColorCounter = g.Sum(x => x.ColorCounter)
                })
                .OrderBy(x => x.Month)
                .ToList();
        }

        public void Compose(IContainer container)
        {
            var maxTotal = Math.Max(_data.Max(x => x.BlackCounter + x.ColorCounter), 1);

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(100); // Mes
                    columns.RelativeColumn();    // Barras
                    columns.ConstantColumn(60);  // Total
                });

                // Leyenda
                table.Cell().ColumnSpan(3).Row(legendRow =>
                {
                    legendRow.ConstantItem(80).Text("Leyenda:").FontSize(9).SemiBold();
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Blue.Darken1);
                    legendRow.AutoItem().Text("B/N").FontSize(9);
                    legendRow.Spacing(10);
                    legendRow.AutoItem().Width(10).Height(10).Background(Colors.Red.Darken1);
                    legendRow.AutoItem().Text("Color").FontSize(9);
                });

                // Encabezados
                table.Cell().Text("Mes").FontSize(10).SemiBold();
                table.Cell().Text("Distribución").FontSize(10).SemiBold();
                table.Cell().Text("Total").FontSize(10).SemiBold();

                // Datos
                foreach (var item in _data)
                {
                    var total = item.BlackCounter + item.ColorCounter;

                    // Mes
                    table.Cell().Text(item.Month).FontSize(10);

                    // Barras apiladas con Stack
                    table.Cell().Height(25).Stack(stack =>
                    {
                        // Fondo gris claro (barra base)
                        stack.Item().Background(Colors.Grey.Lighten3).Height(25);

                        // Barra Negro (superpuesta)
                        if (item.BlackCounter > 0)
                        {
                            stack.Item()
                                .Background(Colors.Blue.Darken1)
                                .Width(item.BlackCounter / (float)maxTotal * 100)
                                .Height(25)
                                .AlignMiddle()
                                .Text(item.BlackCounter.ToString("N0"),
                                    TextStyle.Default.Size(8).Color(Colors.White));
                        }

                        // Barra Color (superpuesta)
                        if (item.ColorCounter > 0)
                        {
                            stack.Item()
                                .Background(Colors.Red.Darken1)
                                .Width(item.ColorCounter / (float)maxTotal * 100)
                                .Height(25)
                                .AlignMiddle()
                                .Text(item.ColorCounter.ToString("N0"),
                                    TextStyle.Default.Size(8).Color(Colors.White));
                        }
                    });

                    // Total
                    table.Cell().Text(total.ToString("N0"))
                             .FontSize(10).SemiBold();
                }
            });
        }
    }
}
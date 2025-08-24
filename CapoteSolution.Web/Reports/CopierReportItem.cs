namespace CapoteSolution.Web.Reports
{
    public class CopierReportItem
    {
        public string PrinterId { get; set; }
        public string Month { get; set; }
        public DateTime Date { get; set; }
        public int BlackCounter { get; set; }
        public int ColorCounter { get; set; }
        public int PlanBw { get; set; }
        public int PlanColor { get; set; }
        public int BlackDifference { get; set; }
        public int ColorDifference { get; set; }
        public int ExtraBlack { get; set; }
        public int ExtraColor { get; set; }
        public int TotalBlack { get; set; }
        public int TotalColor { get; set; }
        public int TotalCopies { get; set; }
        public bool IsMonthlyCounter { get; set; }
        public int? PreviousBlack { get; set; }
        public int? PreviousColor { get; set; }

        // Propiedades calculadas
        public int TotalDifference => BlackDifference + ColorDifference;
        public int TotalExtra => ExtraBlack + ExtraColor;
        public decimal TotalCost => (ExtraBlack * 0.05m) + (ExtraColor * 0.08m); // Ajusta los precios
    }
}

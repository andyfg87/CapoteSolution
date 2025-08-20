namespace CapoteSolution.Web.Reports
{
    public class CopierReportItem
    {
        public int PlanBW { get; set; }
        public int PlanColor { get; set; }
        public string Month { get; set; }     // Ej: "Enero 2023"
        public int BlackCopies { get; set; }  // Copias en B/N
        public int ColorCopies { get; set; }  // Copias en color
    }
}

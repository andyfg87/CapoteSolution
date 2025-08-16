using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class CopierDisplayVM : IEntityDisplayModel<Copier, string>
    {
        public string Id { get; set; }

        [Display(Name = "Número de Serie")]
        public string SerialNumber { get; set; }

        [Display(Name = "Dirección IP")]
        public string IPAddress { get; set; }

        [Display(Name = "Email")]
        public string MachineEmail { get; set; }

        [Display(Name = "Modelo")]
        public string MachineModelInfo { get; set; }

        [Display(Name = "Plan B/N")]
        public int? PlanBW { get; set; }

        [Display(Name = "Plan Color")]
        public int? PlanColor { get; set; }

        [Display(Name = "Precio Mensual")]
        public decimal? MonthlyPrice { get; set; }

        [Display(Name = "Estado Contrato")]
        public string ContractStatus { get; set; }

        [Display(Name = "Cliente")]
        public string CustomerName { get; set; }

        [Display(Name = "Toner")]
        public string TonerName { get; set; }

        [Display(Name = "Marca")]
        public string BrandName { get; set; }
        [Display(Name = "Día de Facturación")]
        public int InvoiceDay { get; set; }
        [Display(Name = "Estado")]
        public string Status { get; set; }
        [Display(Name = "Precio Extra Plan")]
        public decimal? ExtraBW {  get; set; }
        public decimal? ExtraColor {  get; set; }
        public string? LastServiceDate {  get; set; }
        public int? HighestTonerChangeCounter { get; set; } = 0;
        public int? HighestNoChangeTonerCounter { get; set; } = 0;
        public int? TonerYield { get; set; } = 0;
        public int? QtyOfHightestTonerChange { get;set; } = 0;


        public void Import(Copier entity)
        {
            Id = entity.Id;
            SerialNumber = entity.SerialNumber;
            IPAddress = entity.IPAddress;
            MachineEmail = entity.MachineEmail;
            MachineModelInfo = $"{entity.MachineModel?.Name}";
            PlanBW = entity.PlanBW;
            PlanColor = entity.PlanColor;
            MonthlyPrice = entity.MonthlyPrice;
            ContractStatus = entity.Status.ToString();
            InvoiceDay = entity.InvoiceDay;
            CustomerName = entity.Customer?.CustomerName;
            TonerName = entity.MachineModel?.Toner?.Model;
            BrandName = entity.MachineModel?.Brand?.Name;
            Status = entity.Status.ToString();
            ExtraBW = entity.ExtraBW;
            ExtraColor = entity.ExtraColor;
            LastServiceDate = LastService(entity) != null ? LastService(entity).Date.ToString("MM/dd/yyyy") : "Sin Servicio Aún" ;
            HighestTonerChangeCounter = HightTonerChange(entity);
            HighestNoChangeTonerCounter = HightNoChangeToner(entity);
            TonerYield = entity.MachineModel?.Toner?.Yield;
        }

        //Devolver servicio de la ultima fecha 
        private Service LastService(Copier copier)
        {
            if (copier.Services.Count == 0)
                return null;

            var lastService = copier.Services.OrderByDescending(s => s.Date).FirstOrDefault();

            return lastService;
        }
        //Del Servicio mas alto de cambio de toner
        private int HightTonerChange(Copier copier)
        {
            if (copier.Services.Count == 0)
                return 0;

            var lastService = copier.Services.Where(s => s.ServiceReason.Name == ServiceReason.Reasons.TonerChange)
                .Select(c => new { Service = c, TotalCounter = c.BlackCounter + c.ColorCounter})
                .OrderBy(c => c.TotalCounter).LastOrDefault();
            QtyOfHightestTonerChange = lastService?.Service.BlackTonerQty != null  ? lastService?.Service.BlackTonerQty: 0;

            return  lastService != null ? lastService.TotalCounter: 0;
             
        }
        //Del Servicio mas alto de no cambio de toner
        private int HightNoChangeToner(Copier copier)
        {
            if (copier.Services.Count == 0)
                return 0;

            var lastService = copier.Services.Where(s => s.ServiceReason.Name != ServiceReason.Reasons.TonerChange)
                .Select(c => new { Service = c, TotalCounter = c.BlackCounter + c.ColorCounter})
                .OrderBy(c => c.TotalCounter).LastOrDefault();

            return lastService != null ? lastService.TotalCounter : 0;
        }

        public int TotalYield() 
        { 
            return ((int)(TonerYield * QtyOfHightestTonerChange)); 
        }


        public int CheckAt() 
        { 
            return (int)(HighestTonerChangeCounter + TotalYield()); 
        }

        public int ChangeIn() 
        { 
            return (int)(CheckAt() - HighestNoChangeTonerCounter); 
        }

        public decimal TonerLife()
        {
            try
            {
                var totaYield = TotalYield();
                if (totaYield <= 0) return 0m; // Evitar división por cero

                decimal result = (decimal)ChangeIn() / (decimal)totaYield;
                return Math.Round(result, 2, MidpointRounding.AwayFromZero);
            }
            catch
            {
                return 0m; // Manejo de errores
            }
        }
    }
}

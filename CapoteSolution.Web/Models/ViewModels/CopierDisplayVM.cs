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
        public int PlanBW { get; set; }

        [Display(Name = "Plan Color")]
        public int PlanColor { get; set; }

        [Display(Name = "Precio Mensual")]
        public decimal MonthlyPrice { get; set; }

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
        public decimal Extra {  get; set; }

        public void Import(Copier entity)
        {
            Id = entity.Id;
            SerialNumber = entity.SerialNumber;
            IPAddress = entity.IPAddress;
            MachineEmail = entity.MachineEmail;
            MachineModelInfo = $"{entity.MachineModel?.Brand?.Name} {entity.MachineModel?.Name}";
            PlanBW = entity.PlanBW;
            PlanColor = entity.PlanColor;
            MonthlyPrice = entity.MonthlyPrice;
            ContractStatus = entity.Status.ToString();
            InvoiceDay = entity.InvoiceDay;
            CustomerName = entity.Customer?.CustomerName;
            TonerName = entity.MachineModel?.Toner.Model;
            BrandName = entity.MachineModel?.Brand?.Name;
            Status = entity.Status.ToString();
            Extra = entity.ExtraBW;
        }
    }
}

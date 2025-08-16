using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class CopierInputVM : IEntityInputModel<Copier, string>
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "ID de Impresora")]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Número de Serie")]
        public string SerialNumber { get; set; }
       
        [Display(Name = "Dirección IP")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Formato IP inválido")]
        public string? IPAddress { get; set; }

        [EmailAddress]
        [Display(Name = "Email de Notificaciones")]
        public string? MachineEmail { get; set; }

        [Required]
        [Display(Name = "Modelo")]
        public Guid MachineModelId { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public Guid BrandId { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public Guid CustomerId { get; set; }

        [Display(Name = "Comentarios")]
        public string? Comments { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        
        [Display(Name = "Plan B/N")]
        public int? PlanBW { get; set; }

        
        [Display(Name = "Plan Color")]
        public int? PlanColor { get; set; }

        
        [Display(Name = "Precio Extra B/N")]
        public decimal? ExtraBWPrice { get; set; }

        
        [Display(Name = "Precio Extra Color")]
        public decimal? ExtraColorPrice { get; set; }

        [Required]
        [Display(Name = "Día Facturación")]
        public int InvoiceDay { get; set; } = 1;

        
        [Display(Name = "Precio Mensual")]
        public decimal? MonthlyPrice { get; set; }

        [Display(Name = "Cobrar Extras")]
        public bool ChargeExtras { get; set; } = true;

        
        [Display(Name = "Estado")]
        public ContractStatus? Status { get; set; } = ContractStatus.Active;

        // Para dropdown
        public SelectList? AvailableMachineModels { get; set; }
        public SelectList? AvailableBrands { get; set; }
        public SelectList? AvailableCustomers { get; set; }

        public Copier Export()
        {
            var entity = new Copier();

            Merge(entity);

            return entity;
        }

        public void Import(Copier entity)
        {
            Id = entity.Id;
            SerialNumber = entity.SerialNumber;
            IPAddress = entity.IPAddress;
            MachineEmail = entity.MachineEmail;
            MachineModelId = entity.MachineModelId;
            BrandId = entity.MachineModel.BrandId;
            CustomerId = entity.CustomerId;
            Comments = entity.Comments;
            StartDate = entity.StartDate;
            PlanBW = entity.PlanBW;
            PlanColor = entity.PlanColor;
            ExtraBWPrice = entity.ExtraBW;
            ExtraColorPrice = entity.ExtraColor;
            InvoiceDay = entity.InvoiceDay;
            MonthlyPrice = entity.MonthlyPrice;
            ChargeExtras = entity.ChargeExtras;
            Status = entity.Status;
        }

        public void Merge(Copier entity)
        {
            entity.SerialNumber = SerialNumber;
            entity.IPAddress = IPAddress;
            entity.MachineEmail = MachineEmail;
            entity.MachineModelId = MachineModelId;
            //BrandId = entity.MachineModel.BrandId;
            entity.CustomerId = CustomerId;
            entity.Comments = Comments;
            entity.StartDate = StartDate;
            entity.PlanBW = PlanBW;
            entity.PlanColor = PlanColor;
            entity.ExtraBW = ExtraBWPrice;
            entity.ExtraColor = ExtraColorPrice;
            entity.InvoiceDay = InvoiceDay;
            entity.MonthlyPrice = MonthlyPrice;
            entity.ChargeExtras = ChargeExtras;
            entity.Status = Status;
        }
    }
}

using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ContractInputVM : IEntityInputModel<Contract, Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Impresora")]
        public string CopierId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        [Range(1, 100000)]
        public int PlanBW { get; set; }

        [Required]
        [Range(1, 100000)]
        public int PlanColor { get; set; }

        [Required]
        [Range(0.01, 1.00)]
        public decimal ExtraBW { get; set; }

        [Required]
        [Range(0.01, 1.00)]
        public decimal ExtraColor { get; set; }

        [Required]
        [Range(1, 28)]
        public int InvoiceDay { get; set; }

        [Required]
        [Range(1, 10000)]
        public decimal MonthlyPrice { get; set; }

        public bool ChargeExtras { get; set; }
        public string Comments { get; set; }
        public ContractStatus Status { get; set; }

        public SelectList AvailableCopiers { get; set; } // Seleccionar las Impresara disponible (Que CopierID sea null)

        public Contract Export()
        {
           var entity = new Contract();

            Merge(entity);

            return entity;
        }

        public void Import(Contract entity)
        {
            Id = entity.Id;
            CopierId = entity.CopierId;
            StartDate = entity.StartDate;
            PlanBW = entity.PlanBW;
            PlanColor = entity.PlanColor;
            ExtraBW = entity.ExtraBW;
            ExtraColor = entity.ExtraColor;
            InvoiceDay = entity.InvoiceDay;
            MonthlyPrice =entity.MonthlyPrice;
            ChargeExtras = entity.ChargeExtras;
            Comments = entity.Comments;
            Status = Status;
        }

        public void Merge(Contract entity)
        {
            entity.CopierId = CopierId;
            entity.StartDate = StartDate;
            entity.PlanBW = PlanBW;
            entity.PlanColor = PlanColor;
            entity.ExtraBW = ExtraBW;
            entity.ExtraColor = ExtraColor;
            entity.InvoiceDay = InvoiceDay;
            entity.MonthlyPrice = MonthlyPrice;
            entity.ChargeExtras = ChargeExtras;
            entity.Comments = Comments;
            entity.Status = Status;
        }       
    }
}

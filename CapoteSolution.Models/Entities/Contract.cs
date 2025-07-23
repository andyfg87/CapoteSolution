using CapoteSolution.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Entities
{
    public class Contract : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public int PlanBW { get; set; }  // Copias plan B/N
        public int PlanColor { get; set; }  // Copias plan color
        public decimal ExtraBW { get; set; }  // Precio copia extra B/N
        public decimal ExtraColor { get; set; }  // Precio copia extra color
        public int InvoiceDay { get; set; }  // Día de facturación (1-28)
        public decimal MonthlyPrice { get; set; }
        public bool ChargeExtras { get; set; }  // Cambiado de ChargeExtra (string) a bool
        public string Comments { get; set; }

        [Required]
        public ContractStatus Status { get; set; } = ContractStatus.Active;  // Enum: Active/Inactive

        // Relación 1:1 con Copier
        [ForeignKey("Copier")]
        public string CopierId { get; set; }  // Cambiado de MachineID
        public virtual Copier Copier { get; set; }
        //Relación 1:1 con Customer
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public ICollection<Service> Services { get; set; }
    }

    public enum ContractStatus
    {
        Active,
        Inactive
    }
}

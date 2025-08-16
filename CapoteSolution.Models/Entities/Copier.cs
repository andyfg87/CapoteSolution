using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Models.Entities
{
    public class Copier: IEntity<string>
    {       

        [Key]       
        public string Id { get; set; }

        [StringLength(50)]
        public string SerialNumber { get; set; }       
        

        [EmailAddress]
        public string? MachineEmail { get; set; }

        public string? IPAddress { get; set; }
        
        public string? Comments { get; set; }

        //Campos de Contratos que pasan a impresora
        [Required]
        public DateTime StartDate { get; set; }
        public int? PlanBW { get; set; }  // Copias plan B/N
        public int? PlanColor { get; set; }  // Copias plan color
        public decimal? ExtraBW { get; set; }  // Precio copia extra B/N
        public decimal? ExtraColor { get; set; }  // Precio copia extra color
        [Required]
        [Range(1, 31, ErrorMessage = "El día de facturación debe estar entre 1 y 31.")]
        public int InvoiceDay { get; set; }  // Día de facturación (1-28)
        public decimal? MonthlyPrice { get; set; }
        public bool ChargeExtras { get; set; }  // Cambiado de ChargeExtra (string) a bool
        [Required]
        public ContractStatus? Status { get; set; } = ContractStatus.Active;


        // Claves foráneas      
        [Required]
        [ForeignKey("MachineModel")]
        public System.Guid MachineModelId { get; set; }

        [Required]
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }

        // Relaciones       
        public MachineModel MachineModel { get; set; }
        public Customer Customer { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        //public virtual Contract Contract { get; set; }
    }

    public enum ContractStatus
    {
        Active,
        Inactive
    }
}

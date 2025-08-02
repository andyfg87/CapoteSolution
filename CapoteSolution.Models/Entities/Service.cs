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
    public class Service: IEntity<System.Guid>
    {
        [Key]       
        public System.Guid Id { get; set; }// Ok

        [Required]
        public DateTime Date { get; set; }//OK

        //Contadores
        public int BlackCounter { get; set; }//OK
        public int? BlackDiff { get; set; }  // Nullable para cálculos posteriores
        public int? ExtraBW { get; set; }  // Diferencia con el plan

        public int ColorCounter { get; set; } //OK
        public int? ColorDiff { get; set; }
        public int? ExtraColor { get; set; }

        [StringLength(50)]
        public string TicketNumber { get; set; } // OK

        // Toner (solo aplica para ServiceReason.TonerChange)
        public int? BlackTonerQty { get; set; }  // Cantidad de toners cambiados        OK

        

        // Relaciones OK
        [ForeignKey("ServiceReason")]
        public byte ServiceReasonId { get; set; }
        public ServiceReason ServiceReason { get; set; }

        [ForeignKey("Copier ")]
        public string CopierId { get; set; }
        public Copier Copier { get; set; }

        [ForeignKey("Technician")]
        public Guid TechnicianId { get; set; }  // Cambiado de UserID
        public User Technician { get; set; } //OK
    }   
}

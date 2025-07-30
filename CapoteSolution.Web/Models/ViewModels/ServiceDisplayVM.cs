using CapoteSolution.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ServiceDisplayVM : CapoteSolution.Models.Interface.IEntityDisplayModel<Service, System.Guid>
    {      

        public System.Guid Id { get; set; }// Ok
        [Display(Name = "Fecha")]
        public DateTime ServiceDate { get; set; }//OK

        //Contadores
        public int BlackCounter { get; set; }//OK
        //public int? BlackDiff { get; set; }  // Nullable para cálculos posteriores
        public int? ExtraBW { get; set; }  // Diferencia con el plan

        public int ColorCounter { get; set; } //OK        

        [StringLength(50)]
        public string TicketNumber { get; set; } // OK

        // Toner (solo aplica para ServiceReason.TonerChange)
        public int? BlackTonerQty { get; set; }  // Cantidad de toners cambiados        OK
        public string ServiceReason { get; set; }
        public string ContractInfo { get; set; }        
       
        public string TechnicianName { get; set; } //OK


        public void Import(Service entity)
        {
            Id = entity.Id;
            ServiceDate = entity.Date;
            BlackCounter = entity.BlackCounter;
            ColorCounter = entity.ColorCounter;
            TicketNumber = entity.TicketNumber;
            BlackTonerQty = entity.BlackTonerQty;
            ServiceReason = entity.ServiceReason.Name;
            ContractInfo = entity.Contract.Comments;
            TechnicianName = $"{entity.Technician?.FirstName} {entity.Technician?.LastName}";
        }
    }
}

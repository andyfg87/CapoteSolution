using CapoteSolution.Models.Interface;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CapoteSolution.Models.Entities
{
    public class ServiceReason : IEntity<byte>
    {

        public ServiceReason() 
        {          
        }

        [Key]
        public byte Id { get; set; } // Id no es auto generado

        [Required]
        [StringLength(50)]
        public string Name { get; set; }  // Cambiado de VisitReason a Name

        // Relaciones
        public ICollection<Service> Services { get; set; }

        // Valores predefinidos
        public static class Reasons
        {
            public const string TonerChange = "Toner Change";
            public const string MonthlyCounter = "Monthly Counter";
            public const string Maintenance = "Maintenance";
        }
    }
}

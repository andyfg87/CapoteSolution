
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapoteSolution.Models.Entities
{
    public class MachineModel : IEntity<System.Guid>
    {
        public MachineModel() 
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [ForeignKey("Brand")]
        public System.Guid BrandId { get; set; }

        [ForeignKey("Toner")]
        public System.Guid TonerId { get; set; }

        // Propiedades de navegación
        public Brand Brand { get; set; }
        public Toner Toner { get; set; }
        public ICollection<Copier> Copiers { get; set; }
    }
}

using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CapoteSolution.Models.Entities
{    
    public class Toner: IEntity<System.Guid>
    {

        public Toner() 
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public System.Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Range(1, int.MaxValue)]
        public int Yield { get; set; }

        public ICollection<MachineModel> MachineModels { get; set; }
    }
}

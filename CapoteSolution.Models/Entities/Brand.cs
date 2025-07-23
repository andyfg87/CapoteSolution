using CapoteSolution.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Entities
{
    public class Brand : IEntity<System.Guid>
    {
        public Brand() 
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
       
        public ICollection<MachineModel> MachineModels { get; set; }
       
    }
}

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
    public class Customer : IEntity<System.Guid>
    {
        public Customer() 
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string CustomerName { get; set; }
        
        public ICollection<Copier> Copiers { get; set; }// Relacion 1 * Copier
    }
}

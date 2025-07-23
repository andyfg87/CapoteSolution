using CapoteSolution.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Entities
{
    public class User : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        public UserRole Role { get; set; }  // Enum en lugar de string

        // Relación inversa
        public ICollection<Service> Services { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Technician
    }
}

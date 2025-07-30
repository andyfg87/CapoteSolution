using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class UserInputVM: IEntityInputModel<User, Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public SelectList? AvailableRoles { get; set; }        

        public User Export()
        {
            var entity = new User();

            Merge(entity);

            return entity;
        }

        public void Import(User entity)
        {
            Id = entity.Id;
            UserName = entity.Username;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Role = entity.Role;
        }

        public void Merge(User entity)
        {
            entity.Username = UserName;
            entity.FirstName = FirstName;
            entity.LastName = LastName;
            entity.Role = Role;
        }
    }
}

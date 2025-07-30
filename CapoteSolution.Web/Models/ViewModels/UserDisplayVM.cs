using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class UserDisplayVM: IEntityDisplayModel<User, Guid>
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        public void Import(User entity)
        {
            Id = entity.Id;
            UserName = entity.Username;
            FullName = $"{entity.FirstName} {entity.LastName}";
            Role = $"{entity.Role.ToString()}";
        }
    }
}

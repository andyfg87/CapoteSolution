using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class UserDisplayVM
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}

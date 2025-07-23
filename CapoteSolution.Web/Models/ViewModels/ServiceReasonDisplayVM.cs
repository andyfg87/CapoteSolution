using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ServiceReasonDisplayVM : IEntityDisplayModel<ServiceReason, byte>
    {
        public byte Id { get; set; }
        public string Name { get; set; }

        public void Import(ServiceReason entity)
        {
            Id = entity.Id;
            Name = entity.Name;
        }
    }
}

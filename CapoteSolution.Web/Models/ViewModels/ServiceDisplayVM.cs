using CapoteSolution.Models.Entities;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ServiceDisplayVM : CapoteSolution.Models.Interface.IEntityDisplayModel<Service, System.Guid>
    {
        public Guid Id { get; set; }

        public void Import(Service entity)
        {
            throw new NotImplementedException();
        }
    }
}

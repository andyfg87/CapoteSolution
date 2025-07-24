using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class CustomerInputVM : IEntityInputModel<Customer, System.Guid>
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }

        public Customer Export()
        {
            var entity = new Customer();

            Merge(entity);

            return entity;
        }

        public void Import(Customer entity)
        {
            Id = entity.Id;
            CustomerName = entity.CustomerName;
        }

        public void Merge(Customer entity)
        {
            entity.CustomerName = CustomerName;
        }
    }
}

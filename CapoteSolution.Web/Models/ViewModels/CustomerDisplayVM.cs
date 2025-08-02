using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class CustomerDisplayVM : IEntityDisplayModel<Customer, System.Guid>
    {
        public Guid Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;
        public string HasContract { get; set; } = string.Empty;

        public void Import(Customer entity)
        {
            Id = entity.Id;
            CustomerName = entity.CustomerName;
            HasContract = entity.Copiers.Count > 0 ? "Contratado": "Sin Contrato";
        }
    }
}

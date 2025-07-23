using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ServiceReasonInputVM : IEntityInputModel<ServiceReason, byte>
    {
        public byte Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ServiceReason Export() 
        {  
            var entity = new ServiceReason();

            Merge(entity);

            return entity;
        }

        public void Import(ServiceReason entity)
        {
            Id = entity.Id;
            Name = entity.Name;
        }

        public void Merge(ServiceReason entity) => entity.Name = Name;
    }
}

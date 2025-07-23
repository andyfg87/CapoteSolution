using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class BrandDisplayVM : IEntityDisplayModel<Brand, Guid>
    {
        public Guid Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        
        public void Import(Brand entity)
        {
            Id = entity.Id;
            Name = entity.Name;            
        }
    }
}

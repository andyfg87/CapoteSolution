using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class BrandInputVM : IEntityInputModel<Brand, Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        public Brand Export()
        {
            var entity = new Brand();
            Merge(entity);
            return entity;
        }

        public void Import(Brand entity)
        {
            Id = entity.Id;
            Name = entity.Name;
        }

        public void Merge(Brand entity) => entity.Name = Name;
    }
}

using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class MachineModelInputVM : IEntityInputModel<MachineModel, Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public System.Guid BrandId{ get; set; }

        [Required]
        [Display(Name = "Toner")]
        public System.Guid TonerId { get; set; }

        public SelectList? Brands { get; set; }
        public SelectList? Toners { get; set; }

        public MachineModel Export()
        {
            var entity = new MachineModel();

            Merge(entity);

            return entity;
        }

        public void Import(MachineModel entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            BrandId = entity.BrandId;
            TonerId = entity.TonerId;
        }

        public void Merge(MachineModel entity)
        {
            entity.Name = Name;
            entity.BrandId = BrandId;
            entity.TonerId = TonerId;
        }
    }
}

using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class MachineModelDisplayVM : IEntityDisplayModel<MachineModel, Guid>
    {
        public Guid Id { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        public string BrandName { get; set; }
        public string TonerModel { get; set; }

        public void Import(MachineModel entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            BrandName = entity.Brand?.Name;
            TonerModel = entity.Toner?.Model;
        }
    }
}

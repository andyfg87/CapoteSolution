using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class TonerDisplayVM : IEntityDisplayModel<Toner, Guid>
    {
        public Guid Id { get; set; }

        [Display(Name = "Modelo")]
        public string Model { get; set; }

        [Display(Name = "Rendimiento")]
        public int Yield { get; set; }
        

        public void Import(Toner entity)
        {
            Id = entity.Id;
            Model = entity.Model;
            Yield = entity.Yield;            
        }
    }
}

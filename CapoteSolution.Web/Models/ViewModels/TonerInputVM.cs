using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class TonerInputVM : IEntityInputModel<Toner, Guid>
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Modelo")]
        public string Model { get; set; }

        [Required]
        [Range(1000, 100000)]
        [Display(Name = "Rendiemiento (copias)")]
        public int Yield { get; set; }


        public Toner Export()
        {
            var entity = new Toner();

            Merge(entity);

            return entity;
        }

        public void Import(Toner entity)
        {
            Id = entity.Id;
            Model = entity.Model;
            Yield = entity.Yield;
        }

        public void Merge(Toner entity)
        {
            entity.Model = Model;
            entity.Yield = Yield;
        }

        
    }
}

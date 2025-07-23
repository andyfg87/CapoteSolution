using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class CopierInputVM : IEntityInputModel<Copier, string>
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "ID de Impresora")]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Número de Serie")]
        public string SerialNumber { get; set; }

        [Required]
        [Display(Name = "Dirección IP")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "Formato IP inválido")]
        public string IPAddress { get; set; }

        [EmailAddress]
        [Display(Name = "Email de Notificaciones")]
        public string MachineEmail { get; set; }

        [Required]
        [Display(Name = "Modelo")]
        public Guid MachineModelId { get; set; }

        [Display(Name = "Comentarios")]
        public string Comments { get; set; }

        // Para dropdown
        public SelectList? AvailableMachineModels { get; set; }

        public Copier Export()
        {
            var entity = new Copier();

            Merge(entity);

            return entity;
        }

        public void Import(Copier entity)
        {
            Id = entity.Id;
            SerialNumber = entity.SerialNumber;
            IPAddress = entity.IPAddress;
            MachineEmail = entity.MachineEmail;
            MachineModelId = entity.MachineModelId;
            Comments = entity.Comments;
        }

        public void Merge(Copier entity)
        {
            entity.SerialNumber = SerialNumber;
            entity.IPAddress = IPAddress;
            entity.MachineEmail = MachineEmail;
            entity.MachineModelId = MachineModelId;
            entity.Comments = Comments;
        }
    }
}

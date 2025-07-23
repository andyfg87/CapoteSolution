using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;
using System.ComponentModel.DataAnnotations;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class CopierDisplayVM : IEntityDisplayModel<Copier, string>
    {
        public string Id { get; set; }

        [Display(Name = "Número de Serie")]
        public string SerialNumber { get; set; }

        [Display(Name = "Dirección IP")]
        public string IPAddress { get; set; }

        [Display(Name = "Email")]
        public string MachineEmail { get; set; }

        [Display(Name = "Modelo")]
        public string MachineModelInfo { get; set; }

        [Display(Name = "Contrato")]
        public string ContractStatus { get; set; }

        public void Import(Copier entity)
        {
            Id = entity.Id;
            SerialNumber = entity.SerialNumber;
            IPAddress = entity.IPAddress;
            MachineEmail = entity.MachineEmail;
            MachineModelInfo = $"{entity.MachineModel?.Brand?.Name} {entity.MachineModel?.Name}";
            ContractStatus = entity.Contract?.Status.ToString() ?? "Sin Contrato";
        }
    }
}

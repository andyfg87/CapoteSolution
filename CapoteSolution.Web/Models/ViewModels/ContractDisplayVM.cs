using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ContractDisplayVM : IEntityDisplayModel<Contract, Guid>
    {
        public Guid Id { get; set; }
        public string CopierInfo { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }

        public void Import(Contract entity)
        {
            Id = entity.Id;
            CopierInfo = $"{entity.Copier?.SerialNumber} ({entity.Copier?.MachineModel?.Name})";
            StartDate = entity.StartDate;
            Status = entity.Status.ToString();
        }
    }
}

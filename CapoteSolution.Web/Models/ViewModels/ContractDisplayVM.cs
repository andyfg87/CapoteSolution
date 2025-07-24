using CapoteSolution.Models.Entities;
using CapoteSolution.Models.Interface;

namespace CapoteSolution.Web.Models.ViewModels
{
    public class ContractDisplayVM : IEntityDisplayModel<Contract, Guid>
    {
        public Guid Id { get; set; }
        public string CopierInfo { get; set; }
        public string CustomerName { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
        public bool ChargeExtras { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int InvoiceDay { get; set; }

        public void Import(Contract entity)
        {
            Id = entity.Id;
            CopierInfo = $"{entity.Copier?.SerialNumber} ({entity.Copier?.MachineModel?.Name})";
            CustomerName = $"{entity.Customer?.CustomerName ?? string.Empty}";
            StartDate = entity.StartDate;
            Status = entity.Status.ToString();
            ChargeExtras = entity.ChargeExtras;
            MonthlyPrice = entity.MonthlyPrice;
            InvoiceDay = entity.InvoiceDay;
        }
    }
}

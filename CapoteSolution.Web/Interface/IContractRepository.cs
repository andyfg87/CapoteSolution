using CapoteSolution.Models.Entities;

namespace CapoteSolution.Web.Interface
{
    public interface IContractRepository: IEntityRepository<Contract, System.Guid>
    {
        Task<Contract> GetByCopierSerialNumberAsync(string serialNumber);
        Task<IEnumerable<Contract>> GetActiveContractsAsync();
    }
}

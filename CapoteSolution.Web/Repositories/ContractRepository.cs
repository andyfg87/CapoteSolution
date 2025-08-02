using CapoteSolution.Models.EF;
using CapoteSolution.Models.Entities;


namespace CapoteSolution.Web.Repositories
{
    public class ContractRepository : EntityRepository<Contract, Guid>
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }

        /*public int ActiveContracts()
        {
            var contractCount = this.GetAll().Result.Select(c => c.Status == ContractStatus.Active).Count();
            return contractCount;
        }*/
    }
}

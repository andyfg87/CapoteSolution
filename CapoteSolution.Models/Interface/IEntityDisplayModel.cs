using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Interface
{
    public interface IEntityDisplayModel<TEntity, TKey>
    {
        TKey Id { get; set; }
        void Import(TEntity entity);
    }
}

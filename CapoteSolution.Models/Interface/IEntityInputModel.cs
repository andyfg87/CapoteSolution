using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapoteSolution.Models.Interface
{
    public interface IEntityInputModel<TEntity, TKey>
    {
        TKey Id { get; set; }
        TEntity Export();
        void Import(TEntity entity);
        void Merge(TEntity entity);
    }
}

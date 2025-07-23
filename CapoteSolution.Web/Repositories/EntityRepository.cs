using CapoteSolution.Models.EF;
using CapoteSolution.Models.Interface;
using CapoteSolution.Web.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CapoteSolution.Web.Repositories
{
    public class EntityRepository<TEntity, TKey> : IEntityRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public EntityRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity) => await _context.Set<TEntity>().AddAsync(entity);
        public async Task UpdateAsync(TEntity entity) => _context.Set<TEntity>().Update(entity);
        public async Task DeleteAsync(TKey id) => _context.Set<TEntity>().Remove(await GetByIdAsync(id));
        public async Task<TEntity> GetByIdAsync(TKey id) => await _context.Set<TEntity>().FindAsync(id);

        public async  Task<IQueryable<TEntity>> GetAll(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking(); // Mejor rendimiento para consultas de solo lectura

            // Aplicar filtros dinámicos
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Incluir propiedades relacionadas (ej: "Brand,Customer")
            foreach (var includeProperty in includeProperties.Split(
                new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Ordenamiento dinámico
            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }

        public async Task<IQueryable<TEntity>> GetAllWithInclude(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }

        public async Task SaveChangesAsync()
        {
            _context.SaveChanges();
        }

        public async Task<IQueryable<TEntity>> GetAllWithNestedInclude(params string[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TicketControl.Data.Contracts;
using TicketControl.Data.Data;

namespace TicketControl.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AuthDbContext _context;
        private DbSet<T> entities;

        public GenericRepository(AuthDbContext context)
        {
            _context = context;
            entities = context.Set<T>();
        }

        public async Task<ICollection<T>> GetAll(Expression<Func<T, bool>> expression = null)
        {
            if (expression != null)
                return await entities.Where(expression).ToListAsync();
            return await entities.ToListAsync();
        }

        public async Task<T> GetOne(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            return await entities.FirstOrDefaultAsync(expression);
        }

        public async Task<bool> Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await entities.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return await Save();
        }
        public async Task<bool> Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entities.Remove(entity);
            return await Save();
        }
        public async Task<bool> Save()
        {
            //if SaveChanges returns less than one change means something has gone
            //wrong and no changes where saved.
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}

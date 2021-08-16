
using Microsoft.EntityFrameworkCore;
using PestControl.Data.Contracts;
using PestControl.Data.Data;
using PestControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControl.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AuthDbContext _db;

        public CommentRepository(AuthDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CreateAsync(Comment entity)
        {
            await _db.Comments.AddAsync(entity);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(Comment entity)
        {
            _db.Comments.Remove(entity);
            return await SaveAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Comments.AnyAsync(p => p.Id == id);
        }

        public async Task<Comment> FindByIdAsync(int id)
        {
            return await _db.Comments.FindAsync(id);
        }

        public async Task<ICollection<Comment>> GetAllAsync()
        {
            return await _db.Comments.OrderBy(p => p.Date).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        

        public async Task<bool> UpdateAsync(Comment updatedEntity)
        {
            var entity = _db.Comments.Attach(updatedEntity);
            entity.State = EntityState.Modified;
            return await SaveAsync();
        }
    }
}

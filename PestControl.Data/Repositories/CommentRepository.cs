
using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Contracts;
using TicketControl.Data.Data;
using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Repositories
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

        public async Task<IEnumerable<Comment>> GetCommentsByProject(int projectId)
        {
            return await _db.Comments
                .Where(c => c.ProjectId == projectId)
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByTicket(int ticketId)
        {
            return await _db.Comments
                .Where(c => c.TicketId == ticketId)
                .Include(c => c.Author)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUser(string userId)
        {
            return await _db.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.Project!)
                .Include(c => c.Ticket!)
                .ToListAsync();
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

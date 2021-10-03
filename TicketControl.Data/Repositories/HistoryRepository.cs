
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
    public class HistoryRepository : IHistoryRepository
    {
        private readonly AuthDbContext _db;

        public HistoryRepository(AuthDbContext db)
        {
            _db = db;
        }
        public async Task<bool> CreateAsync(History entity)
        {
            await _db.Histories.AddAsync(entity);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(History entity)
        {
            _db.Histories.Remove(entity);
            return await SaveAsync();

        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Histories.AnyAsync(h =>h.HistoryId == id);
        }

        public async Task<History> GetByIdAsync(int id)
        {
            return await _db.Histories
                .Include(h => h.Ticket)
                .Include(h => h.Project)
                .FirstOrDefaultAsync(h => h.HistoryId == id);
        }

        public async Task<ICollection<History>> GetAllAsync()
        {
            return await _db.Histories
                .Include(h => h.Ticket)
                .Include(h => h.Project)
                .OrderBy(h => h.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<History>> GetHistoryByProjectId(int projectId)
        {
            return await _db.Histories
                .Where(h => h.ProjectId == projectId)
                .Include(h => h.Project)
                .ToListAsync();
        }

        public async Task<ICollection<History>> GetHistoryByTicketId(int ticketId)
        {
            return await _db.Histories
                .Where(h => h.TicketId == ticketId)
                .Include(h => h.Ticket)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> UpdateAsync(History updatedEntity)
        {
            var entity = _db.Histories.Attach(updatedEntity);
            entity.State = EntityState.Modified;
            return await SaveAsync();
        }
    }
}

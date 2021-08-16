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
    public class TicketRepository : ITicketRepository
    {
        private readonly AuthDbContext _db;

        public TicketRepository(AuthDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CreateAsync(Ticket entity)
        {
            await _db.Tickets.AddAsync(entity);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(Ticket entity)
        {
            _db.Tickets.Remove(entity);
            return await SaveAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Tickets.AnyAsync(p => p.Id == id);
        }

        public async Task<Ticket> FindByIdAsync(int id)
        {
            return await _db.Tickets.FindAsync(id);
        }

        public async Task<ICollection<Ticket>> GetAllAsync()
        {
            return await _db.Tickets.OrderBy(p => p.DateCreated).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<ICollection<Ticket>> Search(string searchKey)
        {

            var blankSearchKey = string.IsNullOrWhiteSpace(searchKey);

            if (!blankSearchKey)
            {
                searchKey = searchKey.ToLower();
                var result = _db.Tickets
                    .Where(p => p.Title.ToLower().Contains(searchKey)
                    || p.Description.ToLower().Contains(searchKey))
                    .OrderBy(p => p.DateCreated);

                return await result.ToListAsync();

            }

            //if search key was empty return all projects
            return await GetAllAsync();


        }

        public async Task<bool> UpdateAsync(Ticket updatedEntity)
        {
            var entity = _db.Tickets.Attach(updatedEntity);
            entity.State = EntityState.Modified;
            return await SaveAsync();
        }
    }
}

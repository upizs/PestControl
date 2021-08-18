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
            return await _db.Tickets
                .Include(p => p.Project)
                .Include(p=>p.AssignedUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ICollection<Ticket>> GetAllAsync()
        {
            
            return await _db.Tickets
                .Include(p => p.Project)
                .Include(p => p.AssignedUser)
                .OrderBy(p => p.DateCreated)
                .ToListAsync();
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
                    .Include(p => p.Project)
                    .Include(p => p.AssignedUser)
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
            updatedEntity.DateUpdated = DateTimeOffset.Now;
            var entity = _db.Tickets.Attach(updatedEntity);
            entity.State = EntityState.Modified;
            return await SaveAsync();
        }

        //Assign and Remove Users
        public async Task<bool> AssignUser(string userId, int ticketId)
        {
            var ticket = await FindByIdAsync(ticketId);
            ticket.AssignedUserId = userId;
            ticket.DateUpdated = DateTimeOffset.Now;
            ticket.Status = Status.Assigned;
            return await SaveAsync();
        }


        public async Task<bool> RemoveUser(int ticketId)
        {
            var ticket = await FindByIdAsync(ticketId);
            ticket.AssignedUserId = null;
            ticket.DateUpdated = DateTimeOffset.Now;
            ticket.Status = Status.NotAssigned;
            return await SaveAsync();
        }

        //Ticket interaction with project 
        public async Task<int> CountTicketsForProject(int projectId)
        {
            return await _db.Tickets.Where(t => t.ProjectId == projectId).CountAsync();
        }

        public async Task<ICollection<Ticket>> GetAllTicketsForProject(int projectId)
        {
            return await _db.Tickets
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .Include(t=>t.Project)
                .OrderBy(t=> t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByPriority(Priority priority)
        {
            return await _db.Tickets
                .Where(t => t.Priority == priority)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByPriority(Priority priority, int projectId)
        {
            return await _db.Tickets
                .Where(t => t.Priority == priority && t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByStatus(Status status)
        {
            return await _db.Tickets
                .Where(t => t.Status == status)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByStatus(Status status, int projectId)
        {
            return await _db.Tickets
                .Where(t => t.Status == status && t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByUser(string userId)
        {
            return await _db.Tickets
                .Where(t => t.AssignedUserId == userId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByUser(string userId, int projectId)
        {
            return await _db.Tickets
                .Where(t => t.AssignedUserId == userId && t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByType(Types type)
        {
            return await _db.Tickets
                .Where(t => t.Type == type)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByType(Types type, int projectId)
        {
            return await _db.Tickets
                .Where(t => t.Type == type && t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }


    }
}

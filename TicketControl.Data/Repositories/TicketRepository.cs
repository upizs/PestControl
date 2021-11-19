using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Contracts;
using TicketControl.Data.Data;
using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketControl.Data.Repositories
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
            //Have to include all these to make Cascade deletion
            var ticket = await _db.Tickets
                .Include(t=>t.Comments)
                .Include(t => t.Histories)
                .FirstOrDefaultAsync(t => t.Id == entity.Id);
            _db.Tickets.Remove(ticket);
            return await SaveAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Tickets.AnyAsync(p => p.Id == id);
        }

        public async Task<Ticket> GetByIdAsync(int id)
        { 
            //I include no tracking to call the entity again
            //and compare to changed entity
            return await _db.Tickets
                .AsNoTracking()
                .Include(p => p.Project)
                .Include(t => t.Comments)
                .Include(p=>p.AssignedUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ICollection<Ticket>> GetAllAsync()
        {
            
            return await _db.Tickets
                .AsNoTracking()
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
            updatedEntity.DateUpdated = DateTime.Now;
            var entity = _db.Tickets.Attach(updatedEntity);
            entity.State = EntityState.Modified;
            return await SaveAsync();
        }

        //Assign and Remove Users
        public async Task<bool> AssignUser(string userId, int ticketId)
        {
            var ticket = await GetByIdAsync(ticketId);
            ticket.AssignedUserId = userId;
            ticket.DateUpdated = DateTime.Now;
            ticket.Status = Status.Assigned;
            return await SaveAsync();
        }


        public async Task<bool> RemoveUser(int ticketId)
        {
            var ticket = await GetByIdAsync(ticketId);
            ticket.AssignedUserId = null;
            ticket.DateUpdated = DateTime.Now;
            ticket.Status = Status.NotAssigned;
            return await SaveAsync();
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
        /// <summary>
        /// Gets Tickets Assigned to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>ICollection<Ticket></Ticket></returns>
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

        public async Task<ICollection<Ticket>> GetAllNotDoneTickets()
        {
            //Because != didnt work in Where, I have used < operator
            return await _db.Tickets
                .Where(t => (int)t.Status < (int)Status.Done )
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }
        public async Task<ICollection<Ticket>> GetAllNotDoneTicketsForUser(string userId)
        {
            //Because != didnt work in Where, I have used < operator
            return await _db.Tickets
                .Where(t => (int)t.Status < (int)Status.Done 
                                && t.AssignedUserId == userId)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetAllNotDoneTickets(int projectId)
        {
            return await _db.Tickets
                .Where(t => (int)t.Status < (int)Status.Done
                                && t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetTicketsByStatus(Status status, string userId)
        {
            return await _db.Tickets
                .Where(t => t.Status == status && t.AssignedUserId == userId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public ICollection<Ticket> GetAllTicketsForProjects(IEnumerable<Project> projects)
        {
            List<Ticket> allTickets = new();
            foreach (var project in projects)
            {
                var projectTickets = project.Tickets;
                allTickets = allTickets.Concat(projectTickets).ToList();
            }

            return allTickets;
        }

        public ICollection<Ticket> GetTicketsByStatusFromProjects(Status status, IEnumerable<Project> projects)
        {
            List<Ticket> allTickets = new();
            foreach (var project in projects)
            {
                var projectTickets = project.Tickets.Where(t => t.Status == status);
                allTickets = allTickets.Concat(projectTickets).ToList();
            }

            return allTickets;
        }

        public ICollection<Ticket> GetAllNotDoneTicketsFromProjects(IEnumerable<Project> projects)
        {
            List<Ticket> allTickets = new();
            foreach (var project in projects)
            {
                var projectTickets = project.Tickets.Where(t => t.Status < Status.Done);
                allTickets = allTickets.Concat(projectTickets).ToList();
            }

            return allTickets;
        }

        public async Task<ICollection<Ticket>> GetTicketsByPriority(Priority priority, string userId)
        {
            return await _db.Tickets
                .Where(t => t.Priority == priority && t.AssignedUserId == userId)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetAllHighPriorityTickets()
        {
            return await _db.Tickets
                .Where(t => t.Priority > Priority.Medium && t.Status < Status.Done)
                .Include(t => t.AssignedUser)
                .Include(t => t.Project)
                .OrderBy(t => t.DateCreated)
                .ToListAsync();
        }

        public async Task<ICollection<Ticket>> GetAllHighPriorityTickets(string userId)
        {
            return await _db.Tickets
                 .Where(t => t.Priority > Priority.Medium 
                 && t.AssignedUserId == userId && t.Status < Status.Done)
                 .Include(t => t.AssignedUser)
                 .Include(t => t.Project)
                 .OrderBy(t => t.DateCreated)
                 .ToListAsync();
        }
    }
}

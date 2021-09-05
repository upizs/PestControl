using Microsoft.AspNetCore.Identity;
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
    public class ProjectRepository : IProjectRepository
    {
        private readonly AuthDbContext _db;

        public ProjectRepository(AuthDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CreateAsync(Project entity)
        {
            await _db.Projects.AddAsync(entity);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(Project entity)
        {
            //Have to include all these to make Cascade deletion
            var project = await _db.Projects
                .Include(p=>p.Comments)
                .Include(p => p.Histories)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Comments)
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            _db.Projects.Remove(project);
            return await SaveAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Projects.AnyAsync(p => p.Id == id);
        }

        public async Task<Project> FindByIdAsync(int id)
        {
            return await _db.Projects
                .Include(p => p.ApplicationUsers)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ICollection<Project>> GetAllAsync()
        {
            return await _db.Projects
                .Include(p => p.ApplicationUsers)
                .OrderBy(p => p.DateCreated)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<ICollection<Project>> Search(string searchKey)
        {

            var blankSearchKey = string.IsNullOrWhiteSpace(searchKey);

            if (!blankSearchKey)
            {
                searchKey = searchKey.ToLower();
                var result = _db.Projects
                    .Where(p => p.Name.ToLower().Contains(searchKey)
                    || p.Description.ToLower().Contains(searchKey))
                    .Include(p => p.ApplicationUsers)
                    .OrderBy(p => p.DateCreated);

                return await result.ToListAsync();

            }

            //if search key was empty return all projects
            return await GetAllAsync();


        }

        public async Task<bool> UpdateAsync(Project updatedEntity)
        {
            var entity = _db.Projects.Attach(updatedEntity);
            entity.State = EntityState.Modified;
            return await SaveAsync();
        }

        //Assigned User Actions

        public async Task<bool> AssignUser(ApplicationUser user, int projectId)
        {
            var project = await FindByIdAsync(projectId);
            project.ApplicationUsers.Add(user);
            return await SaveAsync();
        }
        public async Task<bool> RemoveUser(ApplicationUser user, int projectId)
        {
            var project = await FindByIdAsync(projectId);
            project.ApplicationUsers.Remove(user);
            return await SaveAsync();
        }

        public async Task<ICollection<Project>> GetProjectsByUser(ApplicationUser user)
        {
            return await _db.Projects
                .Where(p => p.ApplicationUsers.Contains(user))
                .Include(p => p.Comments)
                .Include(p=> p.Tickets)
                .Include(p => p.ApplicationUsers)
                .OrderBy(p => p.DateCreated)
                .ToListAsync();
        }
    }

}

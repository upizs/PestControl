

using System.Collections.Generic;
using System.Threading.Tasks;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.BLL.Managers
{
    public class ProjectManager
    {
        private readonly IProjectRepository _projectRepository;
        private readonly HistoryManager _historyManager;

        public ProjectManager(
            IProjectRepository projectRepository,
            HistoryManager historyManager)
        {
            _projectRepository = projectRepository;
            _historyManager = historyManager;
        }

        #region Standart methods
        public async Task<ICollection<Project>> GetAllProjectsAsync()
        {
            return await _projectRepository.GetAllAsync();
        }
        public async Task<ICollection<Project>> GetProjectsForUserAsync(ApplicationUser user)
        {
            return await _projectRepository.GetProjectsByUser(user);
        }

        public async Task<bool> Create(Project project)
        {
            return await _projectRepository.CreateAsync(project);
        }

        public async Task<bool> Exists(int id)
        {
            return await _projectRepository.ExistsAsync(id);
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Project project)
        {
            return await _projectRepository.DeleteAsync(project);
        }
        public async Task<bool> UpdateAsync(Project project, string userName)
        {
            if (project == null)
                //throw exception
                return false;

            var projectChanges = _historyManager.GetProjectChanges(await GetByIdAsync(project.Id), project, userName);
            await _historyManager.UpdateHistory(projectChanges);
            var result = await _projectRepository.UpdateAsync(project);
            return result;
        }

        public async Task<bool> AssignUserAsync(ApplicationUser assignedUser, int projectId, string changesAuthor)
        {
            var project = await GetByIdAsync(projectId);
            project.ApplicationUsers.Add(assignedUser);
            var result = await UpdateAsync(project, changesAuthor);
            return result;
        }

        public async Task<bool> RemoveUserAsync(ApplicationUser assignedUser, int projectId, string changesAuthor)
        {
            var project = await GetByIdAsync(projectId);
            project.ApplicationUsers.Remove(assignedUser);
            var result = await UpdateAsync(project, changesAuthor);
            return result;
        }
        #endregion


    }
}

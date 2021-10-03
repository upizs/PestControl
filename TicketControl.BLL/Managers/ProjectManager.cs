
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.BLL
{
    public class ProjectManager
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly HistoryManager _historyManager;

        public ProjectManager(ITicketRepository ticketRepository,
            IHistoryRepository historyRepository,
            IProjectRepository projectRepository,
            HistoryManager historyManager)
        {
            _ticketRepository = ticketRepository;
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
            _historyManager = historyManager;
        }
        //TODO: Implement History recording with projects as well
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

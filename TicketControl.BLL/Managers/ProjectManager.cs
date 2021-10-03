
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.BLL
{
    public class ProjectManager
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;

        public ProjectManager(ITicketRepository ticketRepository,
            IHistoryRepository historyRepository,
            IProjectRepository projectRepository)
        {
            _ticketRepository = ticketRepository;
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
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


    }
}

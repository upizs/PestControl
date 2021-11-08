using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketControl.BLL.Models;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.BLL.Managers
{
    public class TicketManager
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly HistoryManager _historyManager;
        private readonly IProjectRepository _projectRepository;
        private readonly Helper _helper;

        public TicketManager(ITicketRepository ticketRepository,
            HistoryManager historyManager,
            IProjectRepository projectRepository)
        {
            _ticketRepository = ticketRepository;
            _historyManager = historyManager;
            _projectRepository = projectRepository;
            _helper = new Helper();

        }
        #region Standart methods
        public async Task<bool> CreateAsync(Ticket ticket)
        {
            return await _ticketRepository.CreateAsync(ticket);
        }
        public async Task<bool> UpdateAsync(Ticket ticket, string userName)
        {
            if (ticket == null)
                //throw exception
                return false;

            var ticketChanges = _historyManager
                .GetTicketChanges(await GetByIdAsync(ticket.Id), ticket, userName);
            await _historyManager.UpdateHistory(ticketChanges);
            ticket.DateUpdated = DateTime.Now;
            var result = await _ticketRepository.UpdateAsync(ticket);
            return result;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await _ticketRepository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(Ticket ticket)
        {
            return await _ticketRepository.DeleteAsync(ticket);
        }
        public async Task<bool> Exists(int id)
        {
            return await _ticketRepository.ExistsAsync(id);
        }
        public async Task<bool> SaveAsync()
        {
            return await _ticketRepository.SaveAsync();
        }
        public async Task<ICollection<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<ICollection<Ticket>> GetAssignedTicketsForUserAsync(ApplicationUser user)
        {
            return await _ticketRepository.GetTicketsByUser(user.Id);
        }

        /// <summary>
        /// Returns all visible tickets for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ICollection<Ticket>> GetAllTicketsForUserAsync(ApplicationUser user)
        {
            //I get all the projects user is assigned to and then get the ticket from them
            var projects = await _projectRepository.GetProjectsByUser(user);
            List<Ticket> tickets = new();
            foreach (var project in projects)
            {
                tickets.AddRange(project.Tickets);
            };

            return tickets;
        }
        /// <summary>
        /// Gets tickets that are marked as Done or Not Assigned
        /// Since they must be processed by Admin
        /// </summary>
        /// <returns>List<Ticket></Ticket></Tickets></returns>
        public async Task<ICollection<Ticket>> GetTicketsForAdminAsync()
        {
            var doneTickets = await _ticketRepository.GetTicketsByStatus(Status.Done);
            var notAssignedTickets = await _ticketRepository.GetTicketsByStatus(Status.NotAssigned);
            return doneTickets.Concat(notAssignedTickets).ToList();
        }

        /// <summary>
        /// Gets ticket statistics for Developers Dashboard
        /// </summary>
        /// <param name="user">User name</param>
        /// <returns>TicketStatistics Object</returns>
        public async Task<UserTicketStatistics> GetStatisticsForUserAsync(ApplicationUser user)
        {
            var statistics = new UserTicketStatistics();

            //Set up priority list
            statistics.Priorities = _helper.GetPriorityNames();
            //Pull the projects with all the tickets included
            var projects = await _projectRepository.GetProjectsByUser(user);
            var tickets = _helper.GetTicketsFromProjects(projects);

            //Find tickets only assigned to this user
            tickets = _helper.GetUserTicketsOnly(tickets, user.Id);
            //Tickets Counted by priority for this user
            statistics.PriorityCounts = _helper.GetCountByPriorities(tickets);
            
            //Generate Statistics
            statistics.AssignedTicketCount = tickets.Count();
            statistics.DoneTicketCount = tickets.Count(t => t.Status == Status.Done);
            statistics.HighPriorityTicketCount = tickets.Count(t => t.Priority > Priority.Medium);
            statistics.DonePercentage = _helper.GetDonePercentage(statistics);

            //Project progress bar (Tickets Counted by status)
            statistics.ProjectsAndTheirTicketsByStatus = 
                _helper.GetStatisticsByStatus(projects);


            return statistics;
        }


        public async Task<bool> AssignUserAsync(ApplicationUser assignedUser, int ticketId, string changesAuthor)
        {

            var ticket = await GetByIdAsync(ticketId);
            ticket.AssignedUser = assignedUser;
            ticket.AssignedUserId = assignedUser.Id;
            ticket.Status = Status.Assigned;
            var result = await UpdateAsync(ticket, changesAuthor);
            return result;
        }

        public async Task<bool> RemoveUserAsync(int ticketId, string changesAuthor)
        {
            var ticket = await GetByIdAsync(ticketId);
            ticket.AssignedUser = null;
            ticket.AssignedUserId = null;
            ticket.Status = Status.NotAssigned;
            var result = await UpdateAsync(ticket, changesAuthor);
            return result;
        }

        #endregion
       

    }
}

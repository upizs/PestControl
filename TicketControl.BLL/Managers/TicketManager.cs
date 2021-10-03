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
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly Helper _helper;

        public TicketManager(ITicketRepository ticketRepository,
            IHistoryRepository historyRepository,
            IProjectRepository projectRepository)
        {
            _ticketRepository = ticketRepository;
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
            _helper = new Helper();

        }
        public async Task<bool> Create(Ticket ticket)
        {
            return await _ticketRepository.CreateAsync(ticket);
        }
        public async Task<bool> UpdateAsync(Ticket ticket, string userName)
        {
            if (ticket == null)
                //throw exception
                return false;

            var ticketChanges = GetTicketChanges(await GetByIdAsync(ticket.Id), ticket, userName);
            await UpdateHistory(ticketChanges);
            
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
        public async Task<bool> SaveAsync()
        {
            return await _ticketRepository.SaveAsync();
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


        #region Ticket History
        public List<History> GetTicketChanges(Ticket originalTicket, Ticket ticketToCompare, string changesAuthor)
        {
            List<History> ticketChanges = new();

            if (originalTicket == null || ticketToCompare == null)
                //throw exception
                return ticketChanges;

            var history = new History
            {
                DateCreated = DateTimeOffset.Now,
                User = changesAuthor,
                TicketId = ticketToCompare.Id
            };

            if (originalTicket.Title.ToLower() != ticketToCompare.Title.ToLower())
                ticketChanges.Add(GetNewTicketHistory(history, "Title", originalTicket.Title, ticketToCompare.Title));
            if (originalTicket.Description.ToLower() != ticketToCompare.Description.ToLower())
                ticketChanges.Add(GetNewTicketHistory(history, "Description", originalTicket.Description, ticketToCompare.Description));
            if (originalTicket.Status != ticketToCompare.Status)
                ticketChanges.Add(GetNewTicketHistory(history, "Status",
                    originalTicket.Status.GetAttribute<DisplayAttribute>().Name,
                    ticketToCompare.Status.GetAttribute<DisplayAttribute>().Name));
            if (originalTicket.Priority != ticketToCompare.Priority)
                ticketChanges.Add(GetNewTicketHistory(history, "Priority",
                    originalTicket.Priority.GetAttribute<DisplayAttribute>().Name,
                    ticketToCompare.Priority.GetAttribute<DisplayAttribute>().Name));
            if (originalTicket.Type != ticketToCompare.Type)
                ticketChanges.Add(GetNewTicketHistory(history, "Type",
                    originalTicket.Type.GetAttribute<DisplayAttribute>().Name,
                    ticketToCompare.Type.GetAttribute<DisplayAttribute>().Name));

            if (originalTicket.AssignedUserId != ticketToCompare.AssignedUserId)
            {
                //These checks are necessary in case there is no Assigned User
                string newUserName = "None";
                string oldUserName = "None";
                if (originalTicket.AssignedUser != null)
                    oldUserName = originalTicket.AssignedUser.UserName;
                if (ticketToCompare.AssignedUser != null)
                    newUserName = ticketToCompare.AssignedUser.UserName;

                ticketChanges.Add(GetNewTicketHistory(history, "Assigned User",
                    oldUserName, newUserName));
            }

            return ticketChanges;

        }

        public History GetNewTicketHistory(History history, string fieldChanged, string oldValue, string newValue)
        {
            return new History
            {
                DateCreated = history.DateCreated,
                FieldChanged = fieldChanged,
                OldValue = oldValue,
                NewValue = newValue,
                TicketId = history.TicketId,
                User = history.User
            };
        }

        public async Task UpdateHistory(List<History> ticketChanges)
        {
            if (ticketChanges == null) return;

            foreach (var change in ticketChanges)
            {
                await _historyRepository.CreateAsync(change);
            }
        }
        #endregion

    }
}

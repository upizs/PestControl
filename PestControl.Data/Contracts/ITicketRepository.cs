using PestControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControl.Data.Contracts
{
    public interface ITicketRepository :IRepositoryBase<Ticket>
    {
        Task<ICollection<Ticket>> Search(string searchKey);
        Task<bool> AssignUser(string userId, int ticketId);
        Task<bool> RemoveUser( int ticketId);

        //Return how many tickets for requested project
        Task<int> CountTicketsForProject(int projectId);
        //Return all the tickets for requested project
        Task<ICollection<Ticket>> GetAllTicketsForProject(int projectId);
        //Return all the tickets by priority (create an overload with project)
        Task<ICollection<Ticket>> GetTicketsByPriority(Priority priority);
        Task<ICollection<Ticket>> GetTicketsByPriority(Priority priority, int projectId);

        //Return all the tickets by status(create an overload with project)
        /// <summary>
        /// Gets all the tickets in system that are not done
        /// Excluding Done and Closed
        /// </summary>
        /// <returns>Collection of tickets</returns>
        Task<ICollection<Ticket>> GetAllNotDoneTickets();
        /// <summary>
        /// Gets all not done tickets for specific project
        /// Excluding Done and Closed
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Collection of tickets</returns>
        Task<ICollection<Ticket>> GetAllNotDoneTickets(int projectId);

        Task<ICollection<Ticket>> GetTicketsByStatus(Status status);
        Task<ICollection<Ticket>> GetTicketsByStatus(Status status, int projectId);
        //Return all the tickets by user(create an overload with project)
        Task<ICollection<Ticket>> GetTicketsByUser(string userId);
        Task<ICollection<Ticket>> GetTicketsByUser(string userId, int projectId);

        //Return all the tickets by type(create an overload with project)
        Task<ICollection<Ticket>> GetTicketsByType(Types type);
        Task<ICollection<Ticket>> GetTicketsByType(Types type, int projectId);


    }
}

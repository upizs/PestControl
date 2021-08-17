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
        //Return all the tickets for requested project
        //Return all the tickets by priority
        //Return all the tickets by status
        //Return all the tickets by user
        //Return all the tickets by type

    }
}

using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Contracts
{
    public interface ICommentRepository : IRepositoryBase<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByProject(int projectId);
        Task<IEnumerable<Comment>> GetCommentsByTicket(int ticketId);
        Task<IEnumerable<Comment>> GetCommentsByUser(string userId);
    }
}

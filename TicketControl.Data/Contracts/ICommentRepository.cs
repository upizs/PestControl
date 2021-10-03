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
        Task<ICollection<Comment>> GetCommentsByProject(int projectId);
        Task<ICollection<Comment>> GetCommentsByTicket(int ticketId);
        Task<ICollection<Comment>> GetCommentsByUser(string userId);
    }
}

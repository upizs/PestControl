using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Contracts
{
    public interface IHistoryRepository : IRepositoryBase<History>
    {
        Task<ICollection<History>> GetHistoryByProjectId(int projectId);
        Task<ICollection<History>> GetHistoryByTicketId(int ticketId);
    }
}

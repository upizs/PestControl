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

    }
}

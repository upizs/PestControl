using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Contracts
{
    public interface IProjectRepository : IRepositoryBase<Project>
    {
        Task<ICollection<Project>> Search(string searchKey);
        Task<bool> AssignUser(ApplicationUser user, int projectId);
        Task<bool> RemoveUser(ApplicationUser user, int projectId);
        Task<ICollection<Project>> GetProjectsByUser(ApplicationUser user);

    }
}

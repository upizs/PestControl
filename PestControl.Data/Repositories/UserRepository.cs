using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Repositories
{
   
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Comments)
                .Include(u=> u.Tickets)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            foreach (var ticket in user.Tickets)
            {
                ticket.Status = Status.NotAssigned;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}

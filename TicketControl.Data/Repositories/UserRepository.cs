using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.Data.Data;

namespace TicketControl.Data.Repositories
{
   
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _db;

        public UserRepository(UserManager<ApplicationUser> userManager,
            AuthDbContext db)
        {
            _userManager = userManager;
            _db = db;
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

        public async Task<ApplicationUser> GetUntrackedUser(string userName)
        {
            return await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}

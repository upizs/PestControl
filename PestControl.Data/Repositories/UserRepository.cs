﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PestControl.Data.Contracts;
using PestControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControl.Data.Repositories
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
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
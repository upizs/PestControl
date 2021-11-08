using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.BLL.Managers;
using TicketControl.Data.Models;
using TicketControl.Tests.Database_Tests.MockData;

namespace TicketControl.Tests.Database_Tests
{
    public static class DeleteData
    {
        public async static Task Delete(UserManager<ApplicationUser> userManager, TicketManager ticketManager, ProjectManager projectManager)
        {
            await DeleteProjectsAsync(projectManager);
            await DeleteUsers(userManager);
            await DeleteTickets(ticketManager);
        }

        private async static Task DeleteTickets(TicketManager ticketManager)
        {
            //This is not very safe. Since if I had additional data I would delete that
            //TODO: Fix this by introducing Generic Repository
            var tickets = await ticketManager.GetAllTicketsAsync();
            if (tickets.Count() <= 0)
            {
                return;
            }
            foreach (var ticket in tickets)
            {
                 await ticketManager.DeleteAsync(ticket); 
                
            }
        }

        private static async Task DeleteProjectsAsync(ProjectManager projectManager)
        {
            var projects = await projectManager.GetAllProjectsAsync(); 
            if (projects.Count() <= 0)
            {
                return;
            }
            foreach (var project in projects)
            {
                await projectManager.DeleteAsync(project);
            }
        }

        private async static Task DeleteUsers(UserManager<ApplicationUser> userManager)
        {
            //This is the best scenario. Because I dont need ID to check if the user exists
            var users = new MockUsers().Users;
            foreach (var user in users)
            {
                if (userManager.Users.Any(u => u.UserName == user.UserName))
                {
                    await userManager.DeleteAsync(user);
                }
            }
        }
    }
}

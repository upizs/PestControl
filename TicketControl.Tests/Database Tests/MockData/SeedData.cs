
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TicketControl.Data.Models;
using TicketControl.BLL.Managers;
using TicketControl.BLL;
using TicketControl.Tests.Database_Tests.MockData;
using System.Linq;

namespace TicketControl.Tests.Database_Tests
{
    public static class SeedData
    {
        public static async Task Seed(UserManager<ApplicationUser> userManager, TicketManager ticketManager, ProjectManager projectManager)
        {
            await SeedProjectsAsync(projectManager);
            await SeedUsersAsync(userManager);
            await SeedTickets(ticketManager, projectManager);

        }

        private async static Task SeedTickets(TicketManager ticketManager, ProjectManager projectManager)
        {
            var mockTickets = new MockTickets();
            var projects = await projectManager.GetAllProjectsAsync();
            //I just use one project to add to the tickets as projectId
            //Otherwise I have db conflicts
            var project = projects.FirstOrDefault();
            foreach (var ticket in mockTickets.Tickets)
            {
                ticket.ProjectId = project.Id;
                await ticketManager.CreateAsync(ticket);
            }
        }

        private static async Task SeedProjectsAsync(ProjectManager projectManager)
        {
            var mockProjects = new MockProjects();
            foreach (var project in mockProjects.Projects)
            {
                await projectManager.Create(project);
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var mockUsers = new MockUsers();
            foreach (var user in mockUsers.Users)
            {
                await userManager.CreateAsync(user);
            }
        }



    }
}

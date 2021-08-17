using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Tickets
{
        public class AssignUserModel : PageModel
        {
            private readonly ITicketRepository _ticketRepository;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IProjectRepository _projectRepository;

            public AssignUserModel(ITicketRepository ticketRepository,
                UserManager<ApplicationUser> userManager,
                IProjectRepository projectRepository)
            {
                _ticketRepository = ticketRepository;
                _userManager = userManager;
                _projectRepository = projectRepository;
            }
            [BindProperty]
            public Ticket Ticket { get; set; }
            public List<ApplicationUser> Users { get; set; }
            //I use single property for User to assign and user to remove
            //Had to use "new" because pageModel already has a User
            public new ApplicationUser User { get; set; }
            [TempData]
            public string Message { get; set; }

            public async Task OnGet(int ticketId)
            {
                Ticket = await _ticketRepository.FindByIdAsync(ticketId);
                if (Ticket == null)
                    RedirectToPage("./NotFound");

                if (Ticket.AssignedUser != null)
                {
                    User = Ticket.AssignedUser;
                }

                await GetUsersNotAssigned();
            }

            //For now only one user can be assigned to ticket
            public async Task<IActionResult> OnPostAssign(string assignedUserId)
            {
                User = await _userManager.FindByIdAsync(assignedUserId);
                //Doesnt post all the Ticket properties, so I have to find it again
                Ticket = await _ticketRepository.FindByIdAsync(Ticket.Id);

                if (User != null)
                {
                    //If the page gets refreshed and same query gets executed
                    //Or if someone tries to add assign user
                    if (Ticket.AssignedUser != null && Ticket.AssignedUser == User)
                        ModelState.AddModelError("", 
                            $"User {Ticket.AssignedUser.UserName} is already assigned to this ticket");
                    else
                    {
                        await _ticketRepository.AssignUser(User.Id, Ticket.Id);
                        Message = $"User {User.UserName} assigned";
                    }
                }
                User = Ticket.AssignedUser;
                await GetUsersNotAssigned();

                return Page();

            }

            //I do not get the User to remove, since there is only one user per ticket.
            //I simply remove the user assigned to the ticket.
            public async Task<IActionResult> OnPostRemove()
            {
                Ticket = await _ticketRepository.FindByIdAsync(Ticket.Id);

                if (Ticket.AssignedUser == null)
                    ModelState.AddModelError("", "There are no users assigned to this ticket");

                else
                {
                    Message = $"User {Ticket.AssignedUser.UserName} removed";
                    await _ticketRepository.RemoveUser( Ticket.Id);
                    
                }
                
                await GetUsersNotAssigned();

                return Page();
            }

            //Helper functions
            //TODO: Will need to update this method so it gets only users assigned to the tickets project
            public async Task GetUsersNotAssigned()
            {
                await GetUsersForTicketsProject();
                if (Ticket.AssignedUser != null)
                    Users.Remove(Ticket.AssignedUser);

            }

            public async Task GetUsersForTicketsProject()
            {
                Users = new();
                var project = await _projectRepository.FindByIdAsync(Ticket.ProjectId);
                foreach (var user in project.ApplicationUsers)
                {
                    Users.Add(await _userManager.FindByIdAsync(user.Id));
                }
            }
        }
    
}

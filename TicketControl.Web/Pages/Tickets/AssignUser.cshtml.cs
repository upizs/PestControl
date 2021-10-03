using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TicketControl.BLL;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;
using TicketControl.BLL.Managers;

namespace TicketControl.Web.Pages.Tickets
{
    [Authorize(Roles ="Admin")]
    public class AssignUserModel : PageModel
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IProjectRepository _projectRepository;
            private readonly TicketManager _ticketManager;

        public AssignUserModel(
                UserManager<ApplicationUser> userManager,
                IProjectRepository projectRepository,
                TicketManager ticketManager)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _ticketManager = ticketManager;
        }
            [BindProperty]
            public Ticket Ticket { get; set; }
            public List<ApplicationUser> Users { get; set; }
            //I use single property for User to assign and user to remove
            //Had to use "new" because pageModel already has a User
            public ApplicationUser AssignedUser{ get; set; }
            [TempData]
            public string Message { get; set; }

            public async Task OnGet(int ticketId)
            {
                Ticket = await _ticketManager.GetByIdAsync(ticketId);
                if (Ticket == null)
                    RedirectToPage("./NotFound");

                if (Ticket.AssignedUser != null)
                {
                    AssignedUser = Ticket.AssignedUser;
                }

                await GetUsersNotAssigned();
            }

            //For now only one user can be assigned to ticket
            public async Task<IActionResult> OnPostAssign(string assignedUserId)
            {
                AssignedUser = await _userManager.FindByIdAsync(assignedUserId);
                //Doesnt post all the Ticket properties, so I have to find it again
                Ticket = await _ticketManager.GetByIdAsync(Ticket.Id);

                if (User != null)
                {
                    //If the page gets refreshed and same query gets executed
                    //Or if someone tries to add assign user
                    //if (Ticket.AssignedUser != null && Ticket.AssignedUser == AssignedUser)
                    //    ModelState.AddModelError("", 
                    //        $"User {Ticket.AssignedUser.UserName} is already assigned to this ticket");
                    
                    await _ticketManager.AssignUserAsync(AssignedUser, Ticket.Id, User.Identity.Name);
                    Message = $"User {AssignedUser.UserName} assigned";
                    
                }
                AssignedUser = Ticket.AssignedUser;
                await GetUsersNotAssigned();

                return Page();

            }

            //I do not get the User to remove, since there is only one user per ticket.
            //I simply remove the user assigned to the ticket.
            public async Task<IActionResult> OnPostRemove()
            {
                Ticket = await _ticketManager.GetByIdAsync(Ticket.Id);

                if (Ticket.AssignedUser == null)
                    ModelState.AddModelError("", "There are no users assigned to this ticket");
                else
                {
                    Message = $"User {Ticket.AssignedUser.UserName} removed";
                    await _ticketManager.RemoveUserAsync(Ticket.Id, User.Identity.Name);
                    
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
                var project = await _projectRepository.GetByIdAsync(Ticket.ProjectId);
                Users = project.ApplicationUsers.ToList();
            }
        }
    
}

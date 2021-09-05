using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Projects
{
    [Authorize(Roles = "Admin")]
    public class AssignUsersModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        //private IEnumerable<ApplicationUser> users;

        public AssignUsersModel(IProjectRepository projectRepository, 
            UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
        }
        [BindProperty]
        public Project Project { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        //I use single property for User to assign and user to remove
        //Had to use "new" because pageModel already has a User
        public new ApplicationUser User { get; set; }
        public IEnumerable<ApplicationUser> AssignedUsers { get; set; }
        public string Message { get; set; }
        

        public async Task OnGet(int projectId)
        {
            Project = await _projectRepository.FindByIdAsync(projectId);
            if(Project == null)
                RedirectToPage("./NotFound");
            
            if (Project.ApplicationUsers.Any())
            {
                AssignedUsers = Project.ApplicationUsers.ToList();
            }

            Users = await GetUsersNotAssigned();
        }

        public async Task<IActionResult> OnPostAssign(string assignedUserId)
        {
            User = await _userManager.FindByIdAsync(assignedUserId);
            //Doesnt post all the Project properties, so I have to find it again
            Project = await _projectRepository.FindByIdAsync(Project.Id);
            
            if (User != null)
            {
                if (Project.ApplicationUsers.Any() && Project.ApplicationUsers.Contains(User))
                    ModelState.AddModelError("", "User already added");
                else
                {
                    await _projectRepository.AssignUser(User, Project.Id);
                    Message = $"User {User.UserName} added";
                }
            }
            AssignedUsers = Project.ApplicationUsers.ToList();
            Users = await GetUsersNotAssigned();

            return Page();

        }

        public async Task<IActionResult> OnPostRemove(string userToRemoveId)
        {
            User = await _userManager.FindByIdAsync(userToRemoveId);
            Project = await _projectRepository.FindByIdAsync(Project.Id);
            

            if (User != null)
            {
                if (Project.ApplicationUsers.Any() && !Project.ApplicationUsers.Contains(User))
                    ModelState.AddModelError("", "User is not assigned on this project");

                else
                {
                    await _projectRepository.RemoveUser(User, Project.Id);
                    Message = $"User {User.UserName} removed";
                }
            }
            if (Project.ApplicationUsers.Any())
            {
                AssignedUsers = Project.ApplicationUsers.ToList();
            }
            Users = await GetUsersNotAssigned();

            return Page();
        }

        //Helper functions
        public async Task<IEnumerable<ApplicationUser>> GetUsersNotAssigned()
        {
            var users = await _userManager.Users.ToListAsync();
            if (Project.ApplicationUsers.Any())
            {
                foreach (var assignedUser in AssignedUsers)
                {
                    if (users.Contains(assignedUser))
                        users.Remove(assignedUser);
                }
            }

            return users;
        }

    }
}

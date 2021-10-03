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
using TicketControl.BLL;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Projects
{
    [Authorize(Roles = "Admin")]
    public class AssignUsersModel : PageModel
    {
        #region Properties and Constructor
        private readonly ProjectManager _projectManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssignUsersModel(ProjectManager projectManager,
            UserManager<ApplicationUser> userManager)
        {
            _projectManager = projectManager;
            _userManager = userManager;
        }
        [BindProperty]
        public Project Project { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        //TODO: Find a better name for universalUser
        public ApplicationUser AssignedUser { get; set; }
        public IEnumerable<ApplicationUser> AssignedUsers { get; set; }
        public string Message { get; set; }
        #endregion

        public async Task OnGet(int projectId)
        {
            Project = await _projectManager.GetByIdAsync(projectId);
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
            AssignedUser = await _userManager.FindByIdAsync(assignedUserId);
            //Doesnt post all the Project properties, so I have to find it again
            Project = await _projectManager.GetByIdAsync(Project.Id);
            
            if (AssignedUser != null)
            {
                if (Project.ApplicationUsers.Any() && Project.ApplicationUsers.Contains(AssignedUser))
                    ModelState.AddModelError("", "User already added");
                else
                {
                    await _projectManager.AssignUserAsync(AssignedUser, Project.Id, User.Identity.Name);
                    Message = $"User {AssignedUser.UserName} added";
                }
            }
            AssignedUsers = Project.ApplicationUsers.ToList();
            Users = await GetUsersNotAssigned();

            return Page();

        }

        public async Task<IActionResult> OnPostRemove(string userToRemoveId)
        {
            AssignedUser = await _userManager.FindByIdAsync(userToRemoveId);
            Project = await _projectManager.GetByIdAsync(Project.Id);
            

            if (AssignedUser != null)
            {
                if (Project.ApplicationUsers.Any() && !Project.ApplicationUsers.Contains(AssignedUser))
                    ModelState.AddModelError("", "User is not assigned on this project");

                else
                {
                    await _projectManager.RemoveUserAsync(AssignedUser, Project.Id, User.Identity.Name);
                    Message = $"User {AssignedUser.UserName} removed";
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

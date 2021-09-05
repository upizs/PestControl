using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Projects
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(IProjectRepository projectRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public Project NewProject { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Error");
            }

            //Admin gets automatically asigned
            NewProject.ApplicationUsers.Add(user);
            NewProject.CreatedById = user.Id;
            NewProject.DateCreated = DateTimeOffset.Now;

            await _projectRepository.CreateAsync(NewProject);
            
            return RedirectToPage("./AssignUsers", new {projectId = NewProject.Id });
        }
    }
}

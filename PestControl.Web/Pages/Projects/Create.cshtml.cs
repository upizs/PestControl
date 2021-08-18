using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Projects
{
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
            else
            {
                //Later on allow only logedin and in role users to create projects
                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    NewProject.CreatedById = user.Id;
                }
                NewProject.DateCreated = DateTimeOffset.Now;
                await _projectRepository.CreateAsync(NewProject);
            }

            return RedirectToPage("./AssignUsers", new {projectId = NewProject.Id });
        }
    }
}

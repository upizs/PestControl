using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;

        public CreateModel(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
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
               NewProject.DateCreated = DateTimeOffset.Now;
               await _projectRepository.CreateAsync(NewProject);
            }

            return RedirectToPage("./AssignUsers", new {projectId = NewProject.Id });
        }
    }
}

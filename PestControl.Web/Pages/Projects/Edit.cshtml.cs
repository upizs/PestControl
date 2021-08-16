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
    public class EditModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;

        public EditModel(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        [BindProperty]
        public Project Project { get; set; }
        public async Task<IActionResult> OnGet(int projectId)
        {
            Project = await _projectRepository.FindByIdAsync(projectId);
            if (Project == null)
                return RedirectToPage("./NotFound");

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            await _projectRepository.UpdateAsync(Project);

            TempData["Message"] = "Project saved!";

            return RedirectToPage("./List");
        }
    }
}

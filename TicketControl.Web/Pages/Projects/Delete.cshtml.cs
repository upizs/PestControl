using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Projects
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;

        public DeleteModel(IProjectRepository projectRepository)
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
            
            //in case of an unexpected error
            if (Project == null)
                return RedirectToPage("./NotFound");

            await _projectRepository.DeleteAsync(Project);
            
            TempData["Message"] = $"Project {Project.Name} Deleted";
            return RedirectToPage("./List");
        }
    }
}

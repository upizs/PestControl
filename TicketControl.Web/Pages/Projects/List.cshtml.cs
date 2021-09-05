using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Projects
{
    public class ListModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListModel(IProjectRepository projectRepository,
            UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
        }

        public IEnumerable<Project> Projects { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                RedirectToPage("/Identity/Login");

            Projects = await _projectRepository.GetProjectsByUser(user);

            return Page();
        }
    }
}

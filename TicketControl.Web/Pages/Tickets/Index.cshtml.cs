using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProjectRepository _projectRepository;

        public IndexModel(ITicketRepository ticketRepository,
            UserManager<ApplicationUser> userManager,
            IProjectRepository projectRepository)
        {
            _ticketRepository = ticketRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
        }
        
        public IEnumerable<Ticket> Tickets { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            var projects = await _projectRepository.GetProjectsByUser(user);
            Tickets =  _ticketRepository.GetAllTicketsForProjects(projects);
        }

        public async Task<IActionResult> OnPostNotDone()
        {
            var user = await _userManager.GetUserAsync(User);
            var projects = await _projectRepository.GetProjectsByUser(user);
            Tickets =  _ticketRepository.GetAllNotDoneTicketsFromProjects(projects);
            return Page();
        }

        public async Task<IActionResult> OnPostDone()
        {
            var user = await _userManager.GetUserAsync(User);
            var projects = await _projectRepository.GetProjectsByUser(user);
            Tickets = _ticketRepository.GetTicketsByStatusFromProjects(Status.Done, projects);
            return Page();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        
        private readonly ITicketRepository _ticketRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHtmlHelper _htmlHelper;

        public CreateModel(ITicketRepository ticketRepository, 
            IProjectRepository projectRepository,
            UserManager<ApplicationUser> userManager,
            IHtmlHelper htmlHelper)
        {
            
            _ticketRepository = ticketRepository;
            _projectRepository = projectRepository;
            _userManager = userManager;
            _htmlHelper = htmlHelper;
        }

        [BindProperty]
        public Ticket NewTicket { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public IEnumerable<Project> Projects { get; set; }

        public async Task OnGet()
        {
            await GetAllListsReady();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (NewTicket == null)
                RedirectToPage("./NotFound");
            if (!ModelState.IsValid)
            {
                await GetAllListsReady();
                return Page();
            }
            else
            {
                NewTicket.DateCreated = DateTimeOffset.Now;
                NewTicket.DateUpdated = NewTicket.DateCreated;
                NewTicket.Status = Status.NotAssigned;
                NewTicket.SubmittedUserId = user.Id;
                await _ticketRepository.CreateAsync(NewTicket);
                TempData["Message"] = "Ticket created!";
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("./AssignUser", new { ticketId = NewTicket.Id });
            }

            return RedirectToPage("./Details", new { ticketId = NewTicket.Id });


        }

        //Fills all the lists before the page loads
        //I put in separate method, because in case sopmething goes wrong and I
        //need to reload the page I also need to reload the lists. 
        public async Task GetAllListsReady()
        {
            var user = await _userManager.GetUserAsync(User);
            Projects = await _projectRepository.GetProjectsByUser(user);
            Types = _htmlHelper.GetEnumSelectList<Types>();
            Priorities = _htmlHelper.GetEnumSelectList<Priority>();
        }
    }
}

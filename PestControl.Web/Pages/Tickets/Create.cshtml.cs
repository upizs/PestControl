using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        
        private readonly ITicketRepository _ticketRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IHtmlHelper _htmlHelper;

        public CreateModel(ITicketRepository ticketRepository, 
            IProjectRepository projectRepository,
            IHtmlHelper htmlHelper)
        {
            
            _ticketRepository = ticketRepository;
            _projectRepository = projectRepository;
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
                //This is where I add user who created the ticket when Identity is fully working
                await _ticketRepository.CreateAsync(NewTicket);
                TempData["Message"] = "Ticket created!";
            }

            return RedirectToPage("./AssignUser", new { ticketId = NewTicket.Id });
        }

        //Fills all the lists before the page loads
        //I put in separate method, because in case sopmething goes wrong and I
        //need to reload the page I also need to reload the lists. 
        public async Task GetAllListsReady()
        {
            Projects = await _projectRepository.GetAllAsync();
            Types = _htmlHelper.GetEnumSelectList<Types>();
            Priorities = _htmlHelper.GetEnumSelectList<Priority>();
        }
    }
}

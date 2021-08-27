using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Tickets
{
    [Authorize(Roles ="Admin")]
    public class EditModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IHtmlHelper _htmlHelper;
        private readonly IProjectRepository _projectRepository;

        public EditModel(ITicketRepository ticketRepository,
            IHtmlHelper htmlHelper,
            IProjectRepository projectRepository)
        {
            _ticketRepository = ticketRepository;
            _htmlHelper = htmlHelper;
            _projectRepository = projectRepository;
        }
        [BindProperty]
        public Ticket Ticket { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }

        public async Task<IActionResult> OnGet(int ticketId)
        {
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);
            if (Ticket == null)
                return RedirectToPage("./NotFound");
            Projects = await _projectRepository.GetAllAsync();
            Priorities = _htmlHelper.GetEnumSelectList<Priority>();
            Types = _htmlHelper.GetEnumSelectList<Types>();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                await _ticketRepository.UpdateAsync(Ticket);
                TempData["Message"] = $"Ticket {Ticket.Title} updated";
                return RedirectToPage("./Index");
            }

            Projects = await _projectRepository.GetAllAsync();
            Priorities = _htmlHelper.GetEnumSelectList<Priority>();
            Types = _htmlHelper.GetEnumSelectList<Types>();
            return Page();
        }

        
    }
}

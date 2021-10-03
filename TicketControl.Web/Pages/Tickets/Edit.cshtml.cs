using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketControl.BLL;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    [Authorize(Roles ="Admin")]
    public class EditModel : PageModel
    {
        private readonly TicketManager _ticketManager;
        private readonly ProjectManager _projectManager;
        private readonly IHtmlHelper _htmlHelper;

        public EditModel(TicketManager ticketManager,
            ProjectManager projectManager,
            IHtmlHelper htmlHelper)
        {
            _ticketManager = ticketManager;
            _projectManager = projectManager;
            _htmlHelper = htmlHelper;
        }
        [BindProperty]
        public Ticket Ticket { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }

        public async Task<IActionResult> OnGet(int ticketId)
        {
            Ticket = await _ticketManager.GetByIdAsync(ticketId);
            if (Ticket == null)
                return RedirectToPage("./NotFound");
            await GetallListsReady();
            return Page();
        }


        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                await _ticketManager.UpdateAsync(Ticket, User.Identity.Name);
                TempData["Message"] = $"Ticket {Ticket.Title} updated";
                return RedirectToPage("./Index");
            }

            await GetallListsReady();
            return Page();
        }

        private async Task GetallListsReady()
        {
            Projects = await _projectManager.GetAllProjectsAsync();
            Priorities = _htmlHelper.GetEnumSelectList<Priority>();
            Types = _htmlHelper.GetEnumSelectList<Types>();
        }

    }
}

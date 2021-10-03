using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    [Authorize(Roles ="Admin")]
    public class DeleteModel : PageModel
    {
        private readonly TicketManager _ticketManager;

        public DeleteModel(TicketManager ticketManager)
        {
            _ticketManager = ticketManager;
        }
        [BindProperty]
        public Ticket Ticket { get; set; }

        public async Task<IActionResult> OnGet(int ticketId)
        {
            Ticket = await _ticketManager.GetByIdAsync(ticketId);
            if (Ticket == null)
                return RedirectToPage("./NotFound");

            return Page();

        }

        public async Task<IActionResult> OnPost(int ticketId)
        {
            if (Ticket == null)
                return RedirectToPage("./NotFound");

            TempData["Message"] = $"Ticket {Ticket.Title} deleted!";
            await _ticketManager.DeleteAsync(Ticket);

            return RedirectToPage("./Index");
        }
    }
}

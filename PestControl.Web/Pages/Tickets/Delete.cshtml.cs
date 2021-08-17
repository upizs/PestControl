using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Tickets
{
    public class DeleteModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;

        public DeleteModel(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        [BindProperty]
        public Ticket Ticket { get; set; }

        public async Task<IActionResult> OnGet(int ticketId)
        {
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);
            if (Ticket == null)
                return RedirectToPage("./NotFound");

            return Page();

        }

        public async Task<IActionResult> OnPost(int ticketId)
        {
            if (Ticket == null)
                return RedirectToPage("./NotFound");

            TempData["Message"] = $"Ticket {Ticket.Title} deleted!";
            await _ticketRepository.DeleteAsync(Ticket);

            return RedirectToPage("./Index");
        }
    }
}

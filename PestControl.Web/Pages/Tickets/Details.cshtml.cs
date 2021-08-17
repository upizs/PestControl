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
    public class DetailsModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;

        public DetailsModel(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }
        public async Task<IActionResult> OnGet(int ticketId)
        {
            //Also Get all the coments for this ticket
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);
            if (Ticket == null)
                RedirectToPage("./NotFound");

            return Page();
        }
    }
}

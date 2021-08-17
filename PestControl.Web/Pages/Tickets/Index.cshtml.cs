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
    public class IndexModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;

        public IndexModel(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
        
        public IEnumerable<Ticket> Tickets { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task OnGet()
        {
            Tickets = await _ticketRepository.GetAllAsync();
        }
    }
}

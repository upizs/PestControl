using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.BLL;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    public class IndexModel : PageModel
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TicketManager _ticketManager;

        public IndexModel(UserManager<ApplicationUser> userManager,
                            TicketManager ticketManager)
        {
            _userManager = userManager;
            _ticketManager = ticketManager;
        }
        
        public IEnumerable<Ticket> Tickets { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("./NotFound");
            
            //Create a JS solution for ticket filtering
            Tickets = await _ticketManager.GetAllTicketsForUserAsync(user);

            return Page();
        }

    }
}

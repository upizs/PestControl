using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    [Authorize]
    public class MyTicketsModel : PageModel
    {
        private readonly TicketManager _ticketManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyTicketsModel(TicketManager ticketManager,
            UserManager<ApplicationUser> userManager)
        {
            _ticketManager =ticketManager ;
            _userManager = userManager;
        }

        public IEnumerable<Ticket> Tickets { get; set; }
        [BindProperty]
        public ApplicationUser PageUser { get; set; }

        public async Task<IActionResult> OnGet()
        {
            PageUser = await _userManager.GetUserAsync(User);
            if (PageUser == null)
                return RedirectToPage("/Identity/Login");

            //Admin has to check and close all the done tickets
            if (User.IsInRole("Admin"))
            {
                Tickets = await _ticketManager.GetTicketsForAdminAsync();
            }
            else
                Tickets = await _ticketManager.GetAssignedTicketsForUserAsync(PageUser);

            return Page();
        }


    }
}

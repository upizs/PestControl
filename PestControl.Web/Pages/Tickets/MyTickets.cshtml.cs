using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Tickets
{
    [Authorize]
    public class MyTicketsModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyTicketsModel(ITicketRepository ticketRepository,
            UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = ticketRepository;
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
                Tickets = await _ticketRepository.GetTicketsByStatus(Status.Done);
                var notAssignedTickets = await _ticketRepository.GetTicketsByStatus(Status.NotAssigned);
                Tickets = Tickets.Concat(notAssignedTickets);

            }
            else
                Tickets = await _ticketRepository.GetTicketsByUser(PageUser.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostNotDone()
        {
            PageUser = await _userManager.GetUserAsync(User);
            if (PageUser == null)
                return RedirectToPage("/Identity/Login");

            Tickets = await _ticketRepository.GetAllNotDoneTicketsForUser(PageUser.Id);
            return Page();
        }

        //Test this action with annonymous
        public async Task<IActionResult> OnPostDone()
        {
            PageUser = await _userManager.GetUserAsync(User);
            if (PageUser == null)
                return RedirectToPage("/Identity/Login");
            Tickets = await _ticketRepository.GetTicketsByStatus(Status.Done, PageUser.Id);
            return Page();
        }
        public async Task<IActionResult> OnPostClosed()
        {
            PageUser = await _userManager.GetUserAsync(User);
            if (PageUser == null)
                return RedirectToPage("/Identity/Login");
            Tickets = await _ticketRepository.GetTicketsByStatus(Status.Closed);
            return Page();
        }

        public async Task<IActionResult> OnPostNotAssigned()
        {
            PageUser = await _userManager.GetUserAsync(User);
            if (PageUser == null)
                return RedirectToPage("/Identity/Login");
            Tickets = await _ticketRepository.GetTicketsByStatus(Status.NotAssigned);
            return Page();
        }
        

    }
}

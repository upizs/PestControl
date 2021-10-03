using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Models;
using TicketControl.BLL.Managers;
using TicketControl.BLL.Models;

namespace TicketControl.Web.Pages.Dashboard
{
    public class DevelopersDashboardModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TicketManager _ticketManager;

        public DevelopersDashboardModel(
            UserManager<ApplicationUser> userManager,
            TicketManager ticketManger)
        {
            
            _userManager = userManager;
            _ticketManager = ticketManger;
        }

        public UserTicketStatistics Statistics { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Identity/Login");

            Statistics = await _ticketManager.GetStatisticsForUserAsync(user);

            return Page();
        }
    }
}

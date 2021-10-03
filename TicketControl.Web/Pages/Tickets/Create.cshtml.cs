using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketControl.BLL;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly ProjectManager _projectManager;
        private readonly TicketManager _ticketManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHtmlHelper _htmlHelper;

        public CreateModel(ProjectManager projectManager,
            TicketManager ticketManager,
            UserManager<ApplicationUser> userManager,
            IHtmlHelper htmlHelper)
        {
            _projectManager = projectManager;
            _ticketManager = ticketManager;
            _userManager = userManager;
            _htmlHelper = htmlHelper;
        }

        [BindProperty]
        public Ticket NewTicket { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public IEnumerable<Project> Projects { get; set; }

        public async Task OnGet()
        {
            await GetAllListsReady();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            #region Validations
            if (user == null)
                return RedirectToPage("/Identity/Login");
            if (NewTicket == null)
                RedirectToPage("./NotFound");
            if (!ModelState.IsValid)
            {
                await GetAllListsReady();
                return Page();
            }
            #endregion
            else
            {
                MapTicketData(user.Id);
                var saved = await _ticketManager.CreateAsync(NewTicket);
                if (saved)
                    TempData["Message"] = "Ticket created!";
                else
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return Page();
                }
            }
            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("./AssignUser", new { ticketId = NewTicket.Id });
            }

            return RedirectToPage("./Details", new { ticketId = NewTicket.Id });
        }

        //Fills all the lists before the page loads
        //I put in separate method, because in case sopmething goes wrong and I
        //need to reload the page I also need to reload the lists. 
        public async Task GetAllListsReady()
        {
            var user = await _userManager.GetUserAsync(User);
            //User can only add tickets to projects its assigned to
            Projects = await _projectManager.GetProjectsForUserAsync(user);
            Types = _htmlHelper.GetEnumSelectList<Types>();
            Priorities = _htmlHelper.GetEnumSelectList<Priority>();
        }
        public  void MapTicketData(string userId)
        {
            NewTicket.DateCreated = DateTime.Now;
            NewTicket.DateUpdated = NewTicket.DateCreated;
            NewTicket.Status = Status.NotAssigned;
            NewTicket.SubmittedUserId = userId;
        }
    }
}

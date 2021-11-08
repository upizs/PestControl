using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.BLL.Managers;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Tickets
{
    public class DetailsModel : PageModel
    {
        private readonly TicketManager _ticketManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly CommentManager _commentManager;

        public DetailsModel(TicketManager ticketManager,
            IUserRepository userRepository,
            CommentManager commentManager,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ticketManager = ticketManager;
            _userRepository = userRepository;
            _commentManager = commentManager;
        }

        
        public Ticket Ticket { get; set; }
        [BindProperty]
        public Comment Comment { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        [TempData]
        public string Message { get; set; }
        
        public async Task<IActionResult> OnGet(int ticketId)
        {
            //Also Get all the coments for this ticket
            Ticket = await _ticketManager.GetByIdAsync(ticketId);
            if (Ticket == null)
                RedirectToPage("./NotFound");
            Comments = await _commentManager.GetCommentsForTicket(ticketId);
            return Page();
        }

        public async Task<IActionResult> OnPostComment(int ticketId)
        {
            Ticket = await _ticketManager.GetByIdAsync(ticketId);
            #region Validation
            if (Ticket == null)
                RedirectToPage("./NotFound");
            //ModelState.IsValid is checking also Comment.Ticket.Title that is unnecassary and empthy
            //But required when creating a project
            //Therefor I will just check if Comment has a message
            if (string.IsNullOrWhiteSpace(Comment.Message))
            {
                ModelState.AddModelError("", "You need to enter some text to make a comment");
            }
            else if (!_signInManager.IsSignedIn(User))
            {
                ModelState.AddModelError("", "You need to be signed in to make a comment");
            }
            #endregion

            else
            {
                Comment.UserId = _userManager.GetUserId(User);
                Comment.Date = DateTime.Now;
                Comment.TicketId = ticketId;
                await _commentManager.CreateAsync(Comment);
            }

            Comments = await _commentManager.GetCommentsForTicket(ticketId);

            //Needed to clean the bound property so that the comment form is empthy
            Comment = new Comment();
            ModelState.Clear();

            //I redirect to the same page using this method, to change the url
            //Had an issue with handler and when refreshing the same comment was posted again.
            return RedirectToPage("./Details", new { ticketId = ticketId });
        }

        public async Task<IActionResult> OnPostChangeStatus(int ticketId, Status status)
        {
            Ticket = await _ticketManager.GetByIdAsync(ticketId);
            //Had to use untracked user because of Entity Framework
            //conflicting with double tracking when updating ticket
            //TODO: Create a logger to see more details about the bug
            var user = await _userRepository.GetUntrackedUser(User.Identity.Name);
            #region Validations
            if (Ticket == null)
                RedirectToPage("./NotFound");
            if (!_signInManager.IsSignedIn(User))
            {
                ModelState.AddModelError("", "You need to be signed in to make a comment.");
            }
            else if (!User.IsInRole("Admin") || user.Id != Ticket.AssignedUserId)
                ModelState.AddModelError("", "You dont have the authorization to edit this ticket.");
            #endregion

            else
            {
                Ticket.Status = status;
                await _ticketManager.UpdateAsync(Ticket, user.UserName);
            }
            Comments = await _commentManager.GetCommentsForTicket(ticketId);
            return Page();
        }

    }
}

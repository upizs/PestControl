using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Tickets
{
    public class DetailsModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ITicketRepository ticketRepository,
            ICommentRepository commentRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = ticketRepository;
            _commentRepository = commentRepository;
            _signInManager = signInManager;
            _userManager = userManager;
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
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);
            if (Ticket == null)
                RedirectToPage("./NotFound");
            Comments = await _commentRepository.GetCommentsByTicket(ticketId);
            return Page();
        }

        public async Task<IActionResult> OnPostComment(int ticketId)
        {
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);

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
            else
            {
                Comment.UserId = _userManager.GetUserId(User);
                Comment.Date = DateTimeOffset.Now;
                Comment.TicketId = ticketId;
                await _commentRepository.CreateAsync(Comment);
            }
            
            Comments = await _commentRepository.GetCommentsByTicket(ticketId);

            //Needed to clean the bound property so that the comment form is empthy
            Comment = new Comment();
            ModelState.Clear();

            //I redirect to the same page using this method, to change the url
            //Had an issue with handler and when refreshing the same comment was posted again.
            return RedirectToPage("./Details", new { ticketId = ticketId });
        }

        public async Task<IActionResult> OnPostDone(int ticketId)
        {
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (Ticket == null)
                RedirectToPage("./NotFound");

            if (!_signInManager.IsSignedIn(User))
            {
                ModelState.AddModelError("", "You need to be signed in to make a comment.");
            }
            else if (user.Id != Ticket.AssignedUserId)
                ModelState.AddModelError("", "You dont have the authorization to edit this ticket.");
            else
            {
                Ticket.Status = Status.Done;
                Ticket.DateUpdated = DateTimeOffset.Now;
                await _ticketRepository.SaveAsync();
               
            }

            Comments = await _commentRepository.GetCommentsByTicket(ticketId);

            return Page();
        }

        public async Task<IActionResult> OnPostInProgress(int ticketId)
        {
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            

            if (Ticket == null)
                RedirectToPage("./NotFound");

            if (!_signInManager.IsSignedIn(User))
            {
                ModelState.AddModelError("", "You need to be signed in to edit this ticket");
            }
            else if (user.Id != Ticket.AssignedUserId)
                ModelState.AddModelError("", "You dont have the authorization to edit this ticket.");
            else
            {
                Ticket.Status = Status.InProgress;
                Ticket.DateUpdated = DateTimeOffset.Now;
                await _ticketRepository.SaveAsync();

            }

            Comments = await _commentRepository.GetCommentsByTicket(ticketId);


            return Page();
        }

        public async Task<IActionResult> OnPostClose(int ticketId)
        {
            Ticket = await _ticketRepository.FindByIdAsync(ticketId);

            if (Ticket == null)
                RedirectToPage("./NotFound");

            if (!_signInManager.IsSignedIn(User))
            {
                ModelState.AddModelError("", "You need to be signed in to edit this ticket");
            }
            else if (!User.IsInRole("Admin"))
                ModelState.AddModelError("", "You dont have the authorization to close this ticket.");
            else
            {
                Ticket.Status = Status.Closed;
                Ticket.DateUpdated = DateTimeOffset.Now;
                await _ticketRepository.SaveAsync();

            }

            Comments = await _commentRepository.GetCommentsByTicket(ticketId);
            return Page();
        }
    }
}

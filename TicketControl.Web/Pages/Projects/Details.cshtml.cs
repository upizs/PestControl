using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Projects
{
    public class DetailsModel : PageModel
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(IProjectRepository projectRepository,
            ITicketRepository ticketRepository,
            ICommentRepository commentRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _ticketRepository = ticketRepository;
            _commentRepository = commentRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [BindProperty]
        public Project Project { get; set; }
        [BindProperty]
        public Comment Comment { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> OnGet(int projectId)
        {
            Project = await _projectRepository.GetByIdAsync(projectId);
            if (Project == null)
                return RedirectToPage("./NotFound");
            Tickets = await _ticketRepository.GetAllTicketsForProject(projectId);
            Comments = await _commentRepository.GetCommentsByProject(projectId);

            return Page();
        }
        
        public async Task<IActionResult> OnPostComment(int projectId)
        {
            Project = await _projectRepository.GetByIdAsync(projectId);
            Tickets = await _ticketRepository.GetAllTicketsForProject(projectId);

            //Validation
            if (Project == null)
                RedirectToPage("./NotFound");
            //ModelState.IsValid is checking also Comment.Project.Name that is unnecassary and empthy
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
                //Create a new comment
                Comment.UserId = _userManager.GetUserId(User);
                Comment.Date = DateTime.Now;
                Comment.ProjectId = projectId;
                await _commentRepository.CreateAsync(Comment);
            }

            //Must Get comments after the new one is added to be responsive
            Comments = await _commentRepository.GetCommentsByProject(projectId);

            //Needed to clean the bound property so that the comment form is empthy
            Comment = new Comment();
            ModelState.Clear();
            //I redirect to the same page using this method, to change the url
            //Had an issue with handler and when refreshing the same comment was posted again.
            return RedirectToPage("./Details", new { projectId = projectId });
        }
    }
}

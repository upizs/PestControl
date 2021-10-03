using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;
using TicketControl.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketControl.Web.ViewComponents
{
    public class AddCommentViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IProjectRepository _projectRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ICommentRepository _commentRepository;

        public AddCommentViewComponent(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IProjectRepository projectRepository,
            ITicketRepository ticketRepository,
            ICommentRepository commentRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _projectRepository = projectRepository;
            _ticketRepository = ticketRepository;
            _commentRepository = commentRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? projectId, int? ticketId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId.Value);
            var model = new AddCommentModel
            {
                ticketId = ticketId.Value,
                projectId = projectId.Value
            };
            return View(model);
        }
    }
}

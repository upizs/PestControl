using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Comments
{
    public class EditModel : PageModel
    {
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ICommentRepository commentRepository, 
            UserManager<ApplicationUser> userManager)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
        }
        [BindProperty]
        public Comment Comment { get; set; }
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnGet(int commentId, string returnUrl)
        {
            Comment = await _commentRepository.FindByIdAsync(commentId);
            returnUrl = returnUrl ?? Url.Content("/Index");
            
            var user = await _userManager.GetUserAsync(User);
            //Prevets users other than comment author accesing this page
            if (user == null 
                || Comment.UserId != user.Id 
                || Comment == null)
                return RedirectToPage("./NotFound", returnUrl);

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPost( string returnUrl=null)
        {
            returnUrl = returnUrl ?? Url.Content("/Index");

            if (string.IsNullOrWhiteSpace(Comment.Message))
                return Page();

            await _commentRepository.UpdateAsync(Comment);

            TempData["Message"] = "Comment edited!";
            return Redirect("~" + returnUrl);
        }
    }
}

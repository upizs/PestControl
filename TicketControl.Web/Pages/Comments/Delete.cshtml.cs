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

namespace TicketControl.Web.Pages.Comments
{
    public class DeleteModel : PageModel
    {
        
        private readonly CommentManager _commentManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(CommentManager commentManager,
            UserManager<ApplicationUser> userManager)
        {
            _commentManager = commentManager;
            _userManager = userManager;
        }
        [BindProperty]
        public Comment Comment { get; set; }
        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnGet(int commentId, string returnUrl)
        {
            Comment = await _commentManager.GetByIdAsync(commentId);
            returnUrl = returnUrl ?? Url.Content("/Index");
            var user = await _userManager.GetUserAsync(User);

            #region Validations
            if (user == null || Comment == null)
                return RedirectToPage("./NotFound", returnUrl);
            //Either Admin or Author can access the page
            else if (User.IsInRole("Admin") || user.Id == Comment.UserId)
            {
                ReturnUrl = returnUrl;
                return Page();
            }
            
            //if we get here, then not authorized
            return RedirectToPage("./NotFound", returnUrl);
            #endregion

        }

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("/Index");
            if (Comment == null)
                return RedirectToPage("./NotFound",returnUrl);

            await _commentManager.DeleteAsync(Comment);
            TempData["Message"] = $"Comment Deleted";

            return Redirect(returnUrl);
        }
    }
    
}

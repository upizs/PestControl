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
    public class EditModel : PageModel
    {
        private readonly CommentManager _commentManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(CommentManager commentManager,
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
            //Prevets users other than comment author accesing this page
            if (Comment == null)
                return RedirectToPage("./NotFound", returnUrl);
            if (user == null)
                return RedirectToPage("/Identity/Login");
            if (user.Id != Comment.UserId)
                return Redirect("~" + returnUrl);
            
            #endregion
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPost( string returnUrl=null)
        {
            returnUrl = returnUrl ?? Url.Content("/Index");
            var user = await _userManager.GetUserAsync(User);
            #region Validations
            if (string.IsNullOrWhiteSpace(Comment.Message))
                return Page();
            if(user == null)
                return RedirectToPage("/Identity/Login");
            if(user.Id != Comment.UserId)
                return Redirect("~" + returnUrl);
            #endregion

            await _commentManager.UpdateAsync(Comment);
            TempData["Message"] = "Comment edited!";
            return Redirect("~" + returnUrl);
        }
    }
}

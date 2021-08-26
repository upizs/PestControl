using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Identity
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordConfirmation(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public string PasswordResetConfirmationUrl { get; private set; }

        public async Task<IActionResult> OnGet(string email, string returnUrl =null)
        {
            if (email == null)
                return RedirectToPage("/Index");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound($"Unable to load user with email '{email}'.");

            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = HttpUtility.UrlEncode(code);
                PasswordResetConfirmationUrl = Url.Page(
                    "/Identity/ResetPassword",
                    pageHandler: null,
                    values: new { userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
}

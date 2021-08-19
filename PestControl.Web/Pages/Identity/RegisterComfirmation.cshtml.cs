using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Identity
{
    [AllowAnonymous]
    public class RegisterComfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public string EmailConfirmationUrl { get; set; }

        public RegisterComfirmationModel(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public async Task<IActionResult> OnGet(string email, string returnUrl = null)
        {
            if (email == null)
                return RedirectToPage("/Index");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound($"Unable to load user with email '{email}'.");

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = HttpUtility.UrlEncode(code);
                EmailConfirmationUrl = Url.Page(
                    "/Identity/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                code = HttpUtility.UrlDecode(code);
                var result = await _userManager.ConfirmEmailAsync(user, code);
            }


            return Page();
        }
    }
}

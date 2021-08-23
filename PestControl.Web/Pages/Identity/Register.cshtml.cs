using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PestControl.Data.Models;
using PestControl.Web.Models;

namespace PestControl.Web.Pages.Identity
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        //TODO: Add email confirmation
        //for now to speed up the process I skip logging or email confirmation
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterUserInputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IActionResult OnGet(string returnUrl = null)
        {
            //Restrict logged in users to use register page
            if (_signInManager.IsSignedIn(User))
                return RedirectToPage("/Index");
            returnUrl = returnUrl ?? Url.Content("/Index");
            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("/Index");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email
                };
                //TODO: assign role in registration

                var result = await _userManager.CreateAsync(user, Input.Password);

                //Email comfirmation is taken out for easier registration
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Developer").Wait();
                    //Email confirmation
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackurl = Url.Page(
                    //    "/Identity/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new {  userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm Your Email ",
                    //    "Please confirm Your account by <a href=" +
                    //    $" {HtmlEncoder.Default.Encode(callbackurl)}> clicking here</a>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        return RedirectToPage("RegisterComfirmation", 
                            new { email = Input.Email, returnUrl = returnUrl });
                    else
                        await _signInManager.SignInAsync(user, isPersistent: false);

                    return Redirect("~" + returnUrl);

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

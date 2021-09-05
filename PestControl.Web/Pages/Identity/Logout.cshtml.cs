using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Identity
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Content("/Index");
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();

            return Page();

        }
    }
}

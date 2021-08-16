using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PestControl.Data.Models;
using PestControl.Web.Models;

namespace PestControl.Web.Pages.Identity
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        //TODO: Add email confirmation
        //for now to speed up the process I skip logging or email confirmation
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public RegisterUserInputModel Input { get; set; }


        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email
                };
                var result = await _userManager.CreateAsync(user, Input.Password);

                //Email comfirmation is taken out for easier registration
                if (result.Succeeded)
                {
                    //TODO: assign role in registration
                   
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("../Index");
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

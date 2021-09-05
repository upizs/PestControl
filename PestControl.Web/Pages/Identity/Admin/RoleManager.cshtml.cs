using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.Web.Pages.Identity.Admin
{
    [Authorize(Roles = "Admin")]
    public class RoleManagerModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;

        public RoleManagerModel(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
        }

        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }


        public ApplicationUser UserToChangeRole { get; set; }
        [BindProperty]
        public string RoleToAssingId { get; set; }

        public async Task OnGet()
        {
            Users = await _userManager.Users.ToListAsync();
            Roles = await _roleManager.Roles.ToListAsync();

        }

        public async Task<IActionResult> OnPost(string userId)
        {
            UserToChangeRole = await _userManager.FindByIdAsync(userId);
            
            var role = await _roleManager.FindByIdAsync(RoleToAssingId);
            if (UserToChangeRole == null)
                return RedirectToPage("/Identity/NotFound");

            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                var roles = await _userManager.GetRolesAsync(UserToChangeRole);
                //in my app user can have only one role.
                if (roles.Count() > 0)
                    await _userManager.RemoveFromRoleAsync(UserToChangeRole, roles.FirstOrDefault());
               await  _userManager.AddToRoleAsync(UserToChangeRole, role.Name);
            }

            Users = await _userManager.Users.ToListAsync();
            Roles = await _roleManager.Roles.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDelete(string userId)
        {
            UserToChangeRole = await _userManager.FindByIdAsync(userId);
            
            if (UserToChangeRole == null)
                return RedirectToPage("/Identity/NotFound");

            await _userRepository.DeleteUserAsync(UserToChangeRole.Id);

            Users = await _userManager.Users.ToListAsync();
            Roles = await _roleManager.Roles.ToListAsync();

            return Page();
        }
    }
}

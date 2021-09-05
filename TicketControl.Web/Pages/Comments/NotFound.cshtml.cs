using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicketControl.Web.Pages.Comments
{
    public class NotFoundModel : PageModel
    {
        public string ReturnUrl { get; set; }
        public void OnGet(string returnUrl = null)
        {

        }
    }
}

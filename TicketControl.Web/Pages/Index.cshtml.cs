
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TicketControl.Web.Pages
{

    /*
     * The plan
     * 
     * Mainly its all about 
     * 
     * Tickets
     * Tables 
     * Comunication
     * 
     * 
     * 1. What needs to be done
     * Admin Dashboard
     * The views need consistent design 
     * Comments need changes tracking
     * Tabbles needs to be done with pagination and filtering and buttons and decent style
     * Or Kanban table
     * Tickets needs a card design
     * Galery for screenshots in tickets details page
     * Formating for texts in the form input (https://summernote.org/)
     * Color palatte for the theme https://colorhunt.co/palette/0820322c394b334756ff4c29
     * Ciolor palette for Priority https://colorhunt.co/palette/a20a0affa36cf6eec9799351
     * Color palatte for Status https://colorhunt.co/palette/206a5d81b214ffcc29f58634
     * API for customers to add tickets
     * Accesable history log
     * Notifications
     * Comunication - like email.
     * 
     * 
     * Notes
     * Dashboard is too filled. Less space for statistics more for comunication
     * Buttons should be the same color unless extreme
     * 
     * 
    */
    #region Hidden Index
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
    #endregion
}

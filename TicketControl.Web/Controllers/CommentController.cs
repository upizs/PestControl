using Microsoft.AspNetCore.Mvc;
using TicketControl.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketControl.Web.Controllers
{
    //I use Controller for comments because I want to have a reusable comment form
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddCommentModel model)
        {

            return RedirectToPage("/Projects/List");
        }
    }
}

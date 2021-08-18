using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PestControl.Web.Models
{
    public class AddCommentModel
    {
        public int projectId { get; set; }
        public int ticketId { get; set; }
        public string CommentText { get; set; }
    }
}

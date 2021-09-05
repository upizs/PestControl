using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Models
{
    //Comments can be used for tickets or projects
    public class Comment
    {
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser Author { get; set; }
        public string UserId { get; set; }
        public int? TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        public int? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        [Required(ErrorMessage ="You need to write something to make a comment")]
        public string Message { get; set; }
        public DateTimeOffset Date { get; set; }

    }
}

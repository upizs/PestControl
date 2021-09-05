using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Projects = new HashSet<Project>();
            this.Tickets = new HashSet<Ticket>();
        }
        [Display(Name ="User Name")]
        public override string UserName { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public IList<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}

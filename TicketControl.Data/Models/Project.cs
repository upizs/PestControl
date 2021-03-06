using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Models
{
    public class Project
    {
        public Project()
        {
            ApplicationUsers = new HashSet<ApplicationUser>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Display(Name = "Assigned Users")]
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [ForeignKey("CreatedById")]
        [Display(Name="Created By")]
        [NotMapped]
        public ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();
        public IList<Comment> Comments { get; set; } = new List<Comment>();
        public IList<History> Histories { get; set; } = new List<History>();

    }
}

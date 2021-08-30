using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControl.Data.Models
{
    public enum Priority
    {
        //I put display names on so I can
        //later get a list of strings
        [Display(Name = "Low")]
        Low,
        [Display(Name = "Medium")]
        Medium,
        [Display(Name = "High")]
        High,
        [Display(Name = "Highest")]
        Highest
    }

    public enum Types
    {
        Error,
        Feature
    }
    public enum Status
    {
        [Display(Name = "Not assigned")]
        NotAssigned,
        [Display(Name = "Assigned")]
        Assigned,
        [Display(Name = "In progress")]
        InProgress,
        [Display(Name = "Done")]
        Done,
        [Display(Name = "Closed")]
        Closed
    }
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public Priority Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public Status Status { get; set; }
        public Types Type { get; set; }
        [Display(Name ="Date Created")]
        public DateTimeOffset DateCreated { get; set; }
        [Display(Name = "Date Last Updated")]
        public DateTimeOffset DateUpdated { get; set; }
        [ForeignKey("AssignedUserId")]
        [Display(Name = "Assigned User")]
        public ApplicationUser AssignedUser { get; set; }
        public string AssignedUserId { get; set; }
        [ForeignKey("SubmittedUserId")]
        [Display(Name = "Submitted By")]
        public ApplicationUser SubmittedByUser { get; set; }
        public string SubmittedUserId { get; set; }
        public IList<Comment> Comments { get; set; } = new List<Comment>();


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControl.Data.Models
{
    public enum Priority
    {
        Low,
        Medium,
        High,
        Highest
    }

    public enum Type
    {
        Error,
        Feature
    }
    public enum Status
    {
        Assigned,
        InProgress,
        PendingApproval,
        Fixed
    }
    public class Ticket
    {
        public int Id { get; set; }
        public Priority Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public Status Status { get; set; }
        public Type Type { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateUpdated { get; set; }
        [ForeignKey("AssignedUserId")]
        public ApplicationUser AssignedUser { get; set; }
        public string AssignedUserId { get; set; }
        [ForeignKey("SubmittedUserId")]
        public ApplicationUser SubmittedByUser { get; set; }
        public string SubmittedUserId { get; set; }
       

    }
}

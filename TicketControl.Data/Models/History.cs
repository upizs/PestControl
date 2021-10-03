using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }
        public int? TicketId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public int? ProjectId { get; set; }
        [Required]
        [StringLength(50)]
        public string FieldChanged { get; set; }
        [Required]
        [StringLength(50)]
        public string OldValue { get; set; }

        [Required]
        [StringLength(50)]
        public string NewValue { get; set; }

        public DateTime DateCreated { get; set; }

        public string User { get; set; }

    }
}

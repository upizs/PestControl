using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.Data.Models;

namespace TicketControl.BLL.Models
{
    public class UserTicketStatistics
    {
        public int AssignedTicketCount { get; set; }
        public int DoneTicketCount { get; set; }
        //Not done
        public int HighPriorityTicketCount { get; set; }
        public List<string> Priorities { get; set; }
        public List<int> PriorityCounts { get; set; }
        public decimal DonePercentage { get; set; }
        public Dictionary<string, Dictionary<Status, decimal>> ProjectsAndTheirTicketsByStatus { get; set; }
    }
}

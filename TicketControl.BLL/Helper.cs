using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TicketControl.BLL.Models;
using TicketControl.Data.Models;

namespace TicketControl.BLL
{
    //Keeps all the helper methods
    public class Helper
    {
        
        public List<string> GetPriorityNames()
        {
            //Get the Priority Enums
            var priorityEnums = Enum.GetValues(typeof(Priority))
                            .Cast<Priority>().ToList();
            var priorityNames = new List<String>();
            //Get the Priority Names
            foreach (var priority in priorityEnums)
            {
                var name = priority.GetAttribute<DisplayAttribute>().Name;
                priorityNames.Add(name);
            }

            return priorityNames;
        }
        public List<int> GetCountByPriorities(List<Ticket> tickets)
        {
            //Get the Priority Enums
            var priorityEnums = Enum.GetValues(typeof(Priority))
                            .Cast<Priority>().ToList();
            var count = new List<int>();
            foreach (var priority in priorityEnums)
            {
                var ticketsByPriority = tickets.Where(t => t.Priority == priority);
                if (ticketsByPriority == null)
                    count.Add(0);
                else
                    count.Add(ticketsByPriority.Count());
            }
            return count;
        }

        /// <summary>
        /// Merges all the tickets from given projects in one list
        /// </summary>
        /// <param name="projects"></param>
        /// <returns>List<Tikcet></Tikcet></returns>
        public List<Ticket> GetTicketsFromProjects(ICollection<Project> projects)
        {
            var tickets = new List<Ticket>();
            foreach (var project in projects)
            {
                tickets.AddRange(project.Tickets);
            }

            return tickets;
        }

        public List<Ticket> GetUserTicketsOnly(List<Ticket> tickets, string userId)
        {
            return tickets
                .Where(t => t.AssignedUserId == userId && t.Status < Status.Closed)
                .ToList();

        }

        public decimal GetDonePercentage(UserTicketStatistics statistics)
        {
            decimal donePercentage = decimal.Zero;
            if (statistics.AssignedTicketCount != 0)
            {
                donePercentage = 
                    ((decimal)statistics.DoneTicketCount / statistics.AssignedTicketCount ) * 100;
                donePercentage = Math.Round(donePercentage, 2);
            }

            return donePercentage;
        }

        public Dictionary<string, Dictionary<Status, decimal>> GetStatisticsByStatus(ICollection<Project> projects)
        {
            Dictionary<string, Dictionary<Status, decimal>> groupedProjects = new();
            foreach (var proj in projects)
            {
                var groupedTickets = proj.Tickets.GroupBy(t => t.Status);
                Dictionary<Status, decimal> ticketsByStatus = new();
                foreach (var group in groupedTickets)
                {
                    ticketsByStatus.Add(
                        group.Key,
                        Math.Round((decimal)group.Count() / proj.Tickets.Count() * 100, 2)
                        );
                }
                groupedProjects.Add(
                    proj.Name,
                    ticketsByStatus
                    );
            }
            return  groupedProjects;
        }



    }
}

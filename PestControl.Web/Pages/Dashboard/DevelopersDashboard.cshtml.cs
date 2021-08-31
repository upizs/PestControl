using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PestControl.Data.Contracts;
using PestControl.Data.Models;

namespace PestControl.Web.Pages.Dashboard
{
    public class DevelopersDashboardModel : PageModel
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProjectRepository _projectRepository;
        private readonly IHtmlHelper _htmlHelper;

        public DevelopersDashboardModel(ITicketRepository ticketRepository,
            UserManager<ApplicationUser> userManager,
            IProjectRepository projectRepository,
            IHtmlHelper htmlHelper)
        {
            _ticketRepository = ticketRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
            _htmlHelper = htmlHelper;
        }
        public int AssignedTicketCount { get; set; }
        public int DoneTicketCount { get; set; }
        //For Admin
        public int AllTicketsNotClosed { get; set; }
        //For Admin
        public int NotAssignedTicketCount { get; set; }
        //Not done
        public int HighPriorityTicketCount { get; set; }
        public List<string> Priorities { get; set; }
        public List<int> PriorityCounts { get; set; }
        public decimal DonePercentage { get; set; }
        public Dictionary<string,Dictionary<Status,decimal>> ProjectsAndTheirTicketsByStatus { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Identity/Login");

            //Set up priority list
            var priorityEnums = Enum.GetValues(typeof(Priority))
                            .Cast<Priority>().ToList();
            Priorities = new();
            foreach (var priority in priorityEnums)
            {
                var name = priority.GetAttribute<DisplayAttribute>().Name;
                Priorities.Add(name);
            }

            
            var projects = await _projectRepository.GetProjectsByUser(user);

            //Can create a method GetStatistics, returns Model called statistics with all the info
            //That way I can create one big query and then sort it. 
            if (User.IsInRole("Admin"))
            {
                var tickets = await _ticketRepository.GetAllAsync();
                AllTicketsNotClosed = tickets.Where(t => t.Status < Status.Closed).Count();
                NotAssignedTicketCount = tickets.Where(t => t.Status == Status.NotAssigned).Count();
                DoneTicketCount = tickets.Where(t => t.Status == Status.Done).Count();
                HighPriorityTicketCount = tickets.Where(t => t.Priority > Priority.Medium 
                                                        && t.Status < Status.Done).Count();
                PriorityCounts = new();
                foreach (var priority in priorityEnums)
                {
                    var ticketsByPriotiry = tickets.Where(t => t.Priority == priority
                                                            && t.Status < Status.Closed);
                    if (ticketsByPriotiry == null)
                        PriorityCounts.Add(0);
                    else
                        PriorityCounts.Add(ticketsByPriotiry.Count());
                }
                //Done percent for Admin (Done/AllTickets)
                if (AllTicketsNotClosed != 0)
                {
                    DonePercentage = ((decimal)DoneTicketCount / AllTicketsNotClosed) * 100;
                    DonePercentage = Math.Round(DonePercentage, 2);
                }
                else
                    DonePercentage = Decimal.Zero;


            }
            else
            {
                var tickets = await _ticketRepository.GetTicketsByUser(user.Id);
                //Esclude closed tickets, otherwise info is confusing
                tickets = tickets.Where(t => t.Status < Status.Closed).ToList();
                AssignedTicketCount = tickets.Count();
                var doneTickets = await _ticketRepository.GetTicketsByStatus(Status.Done, user.Id);

                DoneTicketCount = doneTickets.Count();

                var highTickets = await _ticketRepository.GetAllHighPriorityTickets(user.Id);
                HighPriorityTicketCount = highTickets.Count();
                //Done percent
                if (AssignedTicketCount != 0)
                {
                    DonePercentage = ((decimal)DoneTicketCount / AssignedTicketCount) * 100;
                    DonePercentage = Math.Round(DonePercentage, 2);
                }
                else
                    DonePercentage = Decimal.Zero;



                PriorityCounts = new();
                foreach (var priority in priorityEnums)
                {
                    var ticketsByPriotiry = await _ticketRepository.GetTicketsByPriority(priority, user.Id);
                    if (ticketsByPriotiry == null)
                        PriorityCounts.Add(0);
                    else
                        PriorityCounts.Add(ticketsByPriotiry.Count());
                }
            }

            //Project progress bar
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
            ProjectsAndTheirTicketsByStatus = groupedProjects;

            
            return Page();
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketControl.Data.Contracts;
using TicketControl.Data.Models;

namespace TicketControl.BLL.Managers
{
    public class HistoryManager
    {
        private readonly IHistoryRepository _historyRepository;

        public HistoryManager(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }
        #region Ticket History
        public List<History> GetTicketChanges(Ticket originalTicket, Ticket ticketToCompare, string changesAuthor)
        {
            List<History> ticketChanges = new();

            if (originalTicket == null || ticketToCompare == null)
                //throw exception
                return ticketChanges;

            var history = new History
            {
                DateCreated = DateTime.Now,
                User = changesAuthor,
                TicketId = ticketToCompare.Id
            };

            if (originalTicket.Title.ToLower() != ticketToCompare.Title.ToLower())
                ticketChanges.Add(GetNewTicketHistory(history, "Title", originalTicket.Title, ticketToCompare.Title));
            if (originalTicket.Description.ToLower() != ticketToCompare.Description.ToLower())
            {
                //Considering the description is too important to short it down 
                //and that to store both values in db I dont need, I use a reference instead of values
                var valueReplacement = "Refer to the ticket to see the new description";
                ticketChanges.Add(GetNewTicketHistory(history, "Description", "", valueReplacement));
            }
            if (originalTicket.Status != ticketToCompare.Status)
                ticketChanges.Add(GetNewTicketHistory(history, "Status",
                    originalTicket.Status.GetAttribute<DisplayAttribute>().Name,
                    ticketToCompare.Status.GetAttribute<DisplayAttribute>().Name));
            if (originalTicket.Priority != ticketToCompare.Priority)
                ticketChanges.Add(GetNewTicketHistory(history, "Priority",
                    originalTicket.Priority.GetAttribute<DisplayAttribute>().Name,
                    ticketToCompare.Priority.GetAttribute<DisplayAttribute>().Name));
            if (originalTicket.Type != ticketToCompare.Type)
                ticketChanges.Add(GetNewTicketHistory(history, "Type",
                    originalTicket.Type.GetAttribute<DisplayAttribute>().Name,
                    ticketToCompare.Type.GetAttribute<DisplayAttribute>().Name));

            if (originalTicket.AssignedUserId != ticketToCompare.AssignedUserId)
            {
                //These checks are necessary in case there is no Assigned User
                string newUserName = "None";
                string oldUserName = "None";
                if (originalTicket.AssignedUser != null)
                    oldUserName = originalTicket.AssignedUser.UserName;
                if (ticketToCompare.AssignedUser != null)
                    newUserName = ticketToCompare.AssignedUser.UserName;

                ticketChanges.Add(GetNewTicketHistory(history, "Assigned User",
                    oldUserName, newUserName));
            }

            return ticketChanges;

        }

        public History GetNewTicketHistory(History ticketHistory, string fieldChanged, string oldValue, string newValue)
        {
            return new History
            {
                DateCreated = ticketHistory.DateCreated,
                FieldChanged = fieldChanged,
                OldValue = oldValue,
                NewValue = newValue,
                TicketId = ticketHistory.TicketId,
                User = ticketHistory.User
            };
        }
        #endregion

        #region Project History
        public List<History> GetProjectChanges(Project originalProject, Project projectToCompare, string changesAuthor)
        {
            List<History> projectChanges = new();

            if (originalProject == null || projectToCompare == null)
                //throw exception
                return projectChanges;

            var history = new History
            {
                DateCreated = DateTime.Now,
                User = changesAuthor,
                TicketId = projectToCompare.Id
            };

            if (originalProject.Name.ToLower() != projectToCompare.Name.ToLower())
                projectChanges.Add(GetNewTicketHistory(history, "Title", originalProject.Name, projectToCompare.Name));
            if (originalProject.Description.ToLower() != projectToCompare.Description.ToLower())
            {
                //Considering the description is too important to short it down 
                //and that to store both values in db I dont need, I use a reference instead of values
                var valueReplacement = "Refer to the ticket to see the new description";
                projectChanges.Add(GetNewTicketHistory(history, "Description", "", valueReplacement));
            }
            
            var newUsers = projectToCompare.ApplicationUsers.Except(originalProject.ApplicationUsers);
            var removedUsers = originalProject.ApplicationUsers.Except(projectToCompare.ApplicationUsers);
            if (newUsers.Any())
            {
                foreach (var user in newUsers)
                {
                    projectChanges.Add(GetNewTicketHistory(history, "Assigned Users",
                    "none", user.UserName));
                }
            }
            if (removedUsers.Any())
            {
                foreach (var user in removedUsers)
                {
                    projectChanges.Add(GetNewTicketHistory(history, "Removed Users",
                    user.UserName, "none"));
                }
            }

            return projectChanges;
        }
        public History GetNewProjectHistory(History projectHistory, string fieldChanged, string oldValue, string newValue)
        {
            return new History
            {
                DateCreated = projectHistory.DateCreated,
                FieldChanged = fieldChanged,
                OldValue = oldValue,
                NewValue = newValue,
                ProjectId = projectHistory.ProjectId,
                User = projectHistory.User
            };
        }

        #endregion

        //TODO: Could create separate method
        //for tracking comment deletion, creation and editing
        public async Task UpdateHistory(List<History> changes)
        {
            if (changes == null) return;

            foreach (var change in changes)
            {
                await _historyRepository.CreateAsync(change);
            }
        }


    }
}

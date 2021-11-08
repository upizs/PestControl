using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.Data.Models;

namespace TicketControl.Tests.Database_Tests.MockData
{
    public class MockUsers
    {
        public IList<ApplicationUser> Users { get; private set; }
        public MockUsers()
        {
            Users = new List<ApplicationUser>();

            Users.Add(new ApplicationUser
            {
                UserName = "Brigitte",
                Email = "bmcilwreath0@earthlink.net",
                EmailConfirmed = true
            });
            Users.Add(new ApplicationUser
            {
                UserName = "Irena",
                Email = "ilevee1@latimes.com",
                EmailConfirmed = true
            });
            Users.Add(new ApplicationUser
            {
                UserName = "Nessi",
                Email = "nkirvin3@canalblog.com",
                EmailConfirmed = true
            });
        }
    }
}

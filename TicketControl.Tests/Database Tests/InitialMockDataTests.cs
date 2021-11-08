
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TicketControl.Data.Models;
using TicketControl.Tests.Database_Tests.MockData;
using Xunit;
using Xunit.Abstractions;
using Xunit.Priority;

namespace TicketControl.Tests.Database_Tests
{
    [Collection("InitialMockTests")]
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class InitialMockDataTests : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture fixture;
        private readonly ITestOutputHelper _output;

        public InitialMockDataTests(DatabaseFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            _output = output;
        }

        //I have added Priority so that the first test would fill database and last one clean it
        //The functions had to be async so that I wouldnt have conflicts in Context.
        //Therefor fixture wasnt able to SeedData.
        [Fact, Priority(1)]
        public async void DatabaseIsAccesable()
        {
            await fixture.SeedTestData();
            var tickets = await fixture.TicketManager.GetAllTicketsAsync();
            bool notEmpthy = tickets.Count() > 0;
            Assert.True(notEmpthy);
        }

        [Fact, Priority(5)]
        public async void UsersAreAccesable()
        {
            var users = await fixture.UserManager.Users.ToListAsync();
            bool notEmpthy = users.Count() > 0;

            Assert.True(notEmpthy);
        }

        [Fact,Priority(5)]
        public async void MockUserExists()
        {
            var user = new ApplicationUser
            {
                UserName = "Brigitte",
                Email = "bmcilwreath0@earthlink.net",
                EmailConfirmed = true
            };
            var userFormDb = await fixture.UserManager.FindByNameAsync(user.UserName);

            Assert.NotNull(userFormDb);
        }

        [Fact,Priority(9)]
        public async void MockTicketExists()
        {
            var mockTickets = new MockTickets();
            var mockTicket = mockTickets.Tickets.FirstOrDefault();
            var ticketsFromDb = await fixture.TicketManager.GetAllTicketsAsync();

            bool contains = ticketsFromDb.Any(t=> t.Title == mockTicket.Title);
            await fixture.DeleteTestData();

            Assert.True(contains);
        }
    

    }
}

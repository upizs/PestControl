using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketControl.Data.Models;
using Xunit;

namespace TicketControl.Tests.Database_Tests
{
    public class TicketTests : DatabaseFixture
    {

        [Fact]
        public async void CanCreateATicket()
        {
            var ticket = new Ticket
            {
                Title = "Ticket",
                Description = "This is a Ticket",
                ProjectId = 85
            };
            var result = await TicketRepository.Insert(ticket);
            Assert.True(result);
        }
    }
}

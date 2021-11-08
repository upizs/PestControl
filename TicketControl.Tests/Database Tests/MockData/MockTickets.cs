
using System;
using System.Collections.Generic;
using System.Linq;
using TicketControl.Data.Models;

namespace TicketControl.Tests.Database_Tests.MockData
{
    public class MockTickets
    {
        public IList<Ticket> Tickets { get; private set; }

        public MockTickets()
        {
            Tickets = new List<Ticket>();
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 1",
                Description = "This is a test ticket"
            });
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 2",
                Description = "This is a test ticket"
            });
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 3",
                Description = "This is a test ticket"
            });
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 4",
                Description = "This is a test ticket"
            });
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 5",
                Description = "This is a test ticket"
            });
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 6",
                Description = "This is a test ticket"
            });
            Tickets.Add(new Ticket
            {
                Title = "Test Ticket 7",
                Description = "This is a test ticket"
            });
        }

    }
}

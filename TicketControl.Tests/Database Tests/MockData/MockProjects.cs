

using System.Collections.Generic;
using System.Linq;
using TicketControl.Data.Models;

namespace TicketControl.Tests.Database_Tests.MockData
{
    public class MockProjects
    {
        public IList<Project> Projects { get; private set; }

        public MockProjects()
        {
            //I use hardcoded id, because i know I dont have such. 
            //Turns out SQL doesnt allow to insert data with Id
            //Will fix this issue with GenericRepo
            //TODO: find a better way
            Projects = new List<Project>();
            Projects.Add(new Project
            {
                Name = "TestProject",
                Description = "ThisProject is for test"
            });
            Projects.Add(new Project
            {
                Name = "AnotherTestProject",
                Description = "ThisProject is also for test"
            });
        }
    }
}

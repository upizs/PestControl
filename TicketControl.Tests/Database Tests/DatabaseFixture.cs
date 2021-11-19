
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketControl.BLL.Managers;
using TicketControl.Data.Models;
using TicketControl.Web;
using Xunit;

namespace TicketControl.Tests.Database_Tests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        //I use Jimmy Bogard Respawn to reset the connection
        private readonly Checkpoint _checkpoint;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly WebApplicationFactory<Startup> _factory;

        public DatabaseFixture()
        {
            _factory = new TicketControlTestApplicationFactory();
            _configuration = _factory.Services.GetRequiredService<IConfiguration>();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            _checkpoint = new Checkpoint();
            GetManagers();
        }

        public class TicketControlTestApplicationFactory
            : WebApplicationFactory<Startup>
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ConnectionStrings:TicketControlSql", _connectionString}
                    });
                });
            }

            private readonly string _connectionString = "Data Source=DESKTOP-J0AU7B5\\THISSERVER;Initial Catalog=TicketControl;Integrated Security=True;";
        }
        public TicketManager TicketManager;
        public ProjectManager ProjectManager;
        public UserManager<ApplicationUser> UserManager;

        public void GetManagers()
        {
            var scope = _scopeFactory.CreateScope();
            TicketManager = scope.ServiceProvider.GetRequiredService<TicketManager>();
            ProjectManager = scope.ServiceProvider.GetRequiredService<ProjectManager>();
            UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public async Task SeedTestData()
        {
            await SeedData.Seed(UserManager, TicketManager, ProjectManager);
            
        }
        public async Task DeleteTestData()
        {
            await DeleteData.Delete(UserManager, TicketManager, ProjectManager);
        }

        public Task InitializeAsync()
        => _checkpoint.Reset(_configuration.GetConnectionString("TicketControlSql"));
        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}

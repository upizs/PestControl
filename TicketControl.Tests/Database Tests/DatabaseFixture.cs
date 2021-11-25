
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketControl.Data.Contracts;
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
            GetRepo();
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
        public IGenericRepository<Ticket> TicketRepository {get;set;}

        public void GetRepo()
        {
            var scope = _scopeFactory.CreateScope();
            TicketRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Ticket>>();
        }


        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
        //=> _checkpoint.Reset(_configuration.GetConnectionString("TicketControlSql"));
        public Task DisposeAsync()
        {
            _factory?.Dispose();
            return Task.CompletedTask;
        }
    }
}

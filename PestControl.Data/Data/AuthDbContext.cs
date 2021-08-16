using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PestControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControl.Data.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        //I use model builder to remove all the unnecessary rows (dont see value in them yet) 
        //Found out what Normalized User Name and Email are for, allowed them back in the DB.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.AccessFailedCount)
                                               .Ignore(c => c.LockoutEnabled)
                                               .Ignore(c => c.ConcurrencyStamp)
                                               .Ignore(c => c.PhoneNumber)
                                               .Ignore(c => c.PhoneNumberConfirmed)
                                               .Ignore(c => c.SecurityStamp)
                                               .Ignore(c => c.LockoutEnd)
                                               .Ignore(c => c.TwoFactorEnabled);//and so on...

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");//to change the name of table.

        }
    }
}

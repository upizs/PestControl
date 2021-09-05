using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketControl.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketControl.Data.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<History> Histories { get; set; }

        //I use model builder to remove all the unnecessary rows (dont see value in them yet) 
        //Found out what Normalized Columns, Security Stamp,
        //Concurency are for, allowed them back in the DB.
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.AccessFailedCount)
                                               .Ignore(c => c.LockoutEnabled)
                                               .Ignore(c => c.PhoneNumber)
                                               .Ignore(c => c.PhoneNumberConfirmed)
                                               .Ignore(c => c.LockoutEnd)
                                               .Ignore(c => c.TwoFactorEnabled);//and so on...

            modelBuilder.Entity<ApplicationUser>().ToTable("Users");//to change the name of table.
            //Defining relationship between these entities,
            //Because when Adding a List in User model, I got an exception
            modelBuilder.Entity<ApplicationUser>().HasMany( user => user.Tickets)
                .WithOne(t =>t.AssignedUser).HasForeignKey(t => t.AssignedUserId);
        }
    }
}

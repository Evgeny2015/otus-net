using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Microsoft.Extensions.Logging;
using PromoCodeFactory.DataAccess.Data;
using System.Linq;

namespace PromoCodeFactory.DataAccess.EntityFramework
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }        
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Preference> Preference { get; set; }
        public DbSet<PromoCode> PromoCode { get; set; }
        public DbSet<Role> Role { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Promocode)
                .WithMany(x => x.Customer)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Role);

            modelBuilder.Entity<PromoCode>()
                .HasOne(x => x.Preference);                
            
            modelBuilder.Entity<CustomerPreference>()
                .HasKey(x => new { x.PreferenceId, x.CustomerId });

            modelBuilder.Entity<CustomerPreference>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.CustomerPreferences)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<CustomerPreference>()
                .HasOne(x => x.Preference)
                .WithMany(x => x.CustomerPreferences)
                .HasForeignKey(x => x.PreferenceId);            

            modelBuilder.Entity<Customer>().Property(c => c.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(c => c.LastName).HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(c => c.Email).HasMaxLength(100);

            modelBuilder.Entity<Employee>().Property(c => c.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Employee>().Property(c => c.LastName).HasMaxLength(100);
            modelBuilder.Entity<Employee>().Property(c => c.Email).HasMaxLength(100);

            modelBuilder.Entity<Preference>().Property(c => c.Name).HasMaxLength(100);

            modelBuilder.Entity<PromoCode>().Property(c => c.Code).HasMaxLength(100);
            modelBuilder.Entity<PromoCode>().Property(c => c.ServiceInfo).HasMaxLength(100);
            modelBuilder.Entity<PromoCode>().Property(c => c.PartnerName).HasMaxLength(100);

            modelBuilder.Entity<Role>().Property(c => c.Description).HasMaxLength(100);
            modelBuilder.Entity<Role>().Property(c => c.Name).HasMaxLength(20);


            modelBuilder.Entity<Role>().HasData(FakeDataFactory.Roles);
            modelBuilder.Entity<Employee>().HasData(
                FakeDataFactory.Employees.Select(x => 
                    new {
                        x.Id,
                        x.FirstName,
                        x.LastName,                    
                        x.Email,
                        x.AppliedPromocodesCount,
                        RoleId = x.Role.Id 
                    })
                );

            modelBuilder.Entity<Preference>().HasData(FakeDataFactory.Preferences);
            modelBuilder.Entity<Customer>().HasData(FakeDataFactory.Customers);
            modelBuilder.Entity<CustomerPreference>().HasData(
                FakeDataFactory.CustomerPreferences.Select(x =>
                    new CustomerPreference
                    {
                        CustomerId = x.Customer.Id,
                        PreferenceId = x.Preference.Id,
                    }
                ));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}

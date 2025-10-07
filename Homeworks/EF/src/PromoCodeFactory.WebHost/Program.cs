using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace PromoCodeFactory.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();                
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                //db.Database.Migrate();
                //Seed(scope.ServiceProvider);
            }

                host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        protected static void AppendEntity<T>(DataContext context, IEnumerable<T> entity) where T : class
        {            
            var dbSet = context.Set<T>();
            dbSet.AddRange(entity);                        
        }

        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();            

            
            AppendEntity(context, FakeDataFactory.Employees);

            context.SaveChanges();
        }
    }
}
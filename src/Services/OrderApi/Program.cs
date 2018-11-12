﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShoesOnContainers.Services.OrderApi.Data;
using System;

namespace ShoesOnContainers.Services.OrderApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<OrdersContext>();
                    MigrateDatabase.EnsureCreated(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .ConfigureLogging((context, builder) =>
                 {
                     builder.AddConsole();
                     builder.AddDebug();
                 })
                .UseStartup<Startup>()
                .Build();
    }
}
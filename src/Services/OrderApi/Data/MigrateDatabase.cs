using Microsoft.EntityFrameworkCore;

namespace ShoesOnContainers.Services.OrderApi.Data
{
    public static class MigrateDatabase
    {
        public static void EnsureCreated(OrdersContext context)
        {
            System.Console.WriteLine("Migration taking place....Creating database...");
            context.Database.Migrate();
            // RelationalDatabaseCreator databaseCreator =(RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            //  databaseCreator.CreateTables();

            System.Console.WriteLine("Migrations  complete.....");
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UltimatePOS.Data;
using System;
using System.IO;

namespace UltimatePOS.Migrator
{
    public class MigratorDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // Should match the path used in Program.cs
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbDirectory = Path.Combine(appDataPath, "UltimatePOS");
            Directory.CreateDirectory(dbDirectory);
            var dbPath = Path.Combine(dbDirectory, "ultimatepos.db");
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}")
                .EnableSensitiveDataLogging();

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace UltimatePOS.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read connection string
        // We need to look for appsettings.json in the WinUI project or current directory
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Hardcoded fallback for design-time if file not found, or try to find it relative to execution
        var basePath = Directory.GetCurrentDirectory();
        
        // Adjust path if running from solution root vs project folder
        // For CLI tools, typically running from solution root or startup project
        
        var connectionString = "Data Source=UltimatePOS_Design.db";

        optionsBuilder.UseSqlite(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

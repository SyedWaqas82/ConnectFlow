using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ConnectFlow.Infrastructure.Data;

namespace ConnectFlow.Web.Common
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args) // neccessary for EF migration designer to run on this context
        {
            // Build the configuration by reading from the appsettings.json file (requires Microsoft.Extensions.Configuration.Json Nuget Package)
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddUserSecrets(Assembly.GetExecutingAssembly()!, false)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            // Retrieve the connection string from the configuration
            string? connectionString = configuration.GetConnectionString("ConnectFlowDb");


            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
            _ = optionsBuilder.UseNpgsql(connectionString);
            return new ApplicationDbContext(new DesignTimeServiceProvider(), optionsBuilder.Options);
        }

        public class DesignTimeServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }
    }
}
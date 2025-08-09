using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ConnectFlow.Infrastructure.Data;
using ConnectFlow.Application.Common.Interfaces;

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

            // Register dummy services for design-time
            var services = new ServiceCollection();
            services.AddSingleton<ICurrentTenantService, DesignTimeTenantService>();
            services.AddSingleton<ICurrentUserService, DesignTimeUserService>();
            var serviceProvider = services.BuildServiceProvider();

            return new ApplicationDbContext(serviceProvider, optionsBuilder.Options);
        }
    }

    public class DesignTimeTenantService : ICurrentTenantService
    {
        public int? GetCurrentTenantId() => null; // or a default tenant id for migrations
    }

    public class DesignTimeUserService : ICurrentUserService
    {
        public Guid? GetCurrentUserId() => null;
        public int? GetCurrentApplicationUserId() => null;
        public string? GetCurrentUserName() => null;
        public List<string> GetCurrentUserRoles() => new();
        public bool IsSuperAdmin() => false;
    }
}
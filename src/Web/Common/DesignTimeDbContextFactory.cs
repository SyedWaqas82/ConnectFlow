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
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        //     // Stub implementation for ICurrentTenantService
        //     public class DesignTimeTenantService : ICurrentTenantService
        //     {
        //         public Task ClearCurrentTenantAsync()
        //         {
        //             throw new NotImplementedException();
        //         }

        //         public Task<Tenant?> GetCurrentTenantAsync()
        //         {
        //             throw new NotImplementedException();
        //         }

        //         // Implement required members with dummy logic
        //         public string GetCurrentTenantId() => "design-time-tenant-id"; // Dummy tenant ID for design time
        //         public Task<string> GetCurrentTenantIdAsync(CancellationToken cancellationToken = default) => Task.FromResult("design-time-tenant-id");

        //         public Task<int?> GetCurrentTenantIdAsync()
        //         {
        //             throw new NotImplementedException();
        //         }

        //         public bool IsSuperAdmin()
        //         {
        //             throw new NotImplementedException();
        //         }

        //         public Task SetCurrentTenantIdAsync(int tenantId)
        //         {
        //             throw new NotImplementedException();
        //         }
        //     }
        // }
    }
}
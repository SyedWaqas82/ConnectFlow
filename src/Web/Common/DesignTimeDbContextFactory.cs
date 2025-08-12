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
            services.AddSingleton<IContextManager, DesignTimeContextService>();
            var serviceProvider = services.BuildServiceProvider();

            return new ApplicationDbContext(serviceProvider, optionsBuilder.Options);
        }
    }

    public class DesignTimeContextService : IContextManager
    {
        public int? GetCurrentTenantId() => null; // or a default tenant id for migrations
        public Guid? GetCurrentUserId() => null;
        public int? GetCurrentApplicationUserId() => null;
        public string? GetCurrentUserName() => null;
        public List<string> GetCurrentUserRoles() => new();
        public bool IsSuperAdmin() => false;

        public Task InitializeContextAsync(int applicationUserId, int? tenantId)
        {
            throw new NotImplementedException();
        }

        public Task InitializeContextWithDefaultTenantAsync(int applicationUserId)
        {
            throw new NotImplementedException();
        }

        public void SetContext(int? applicationUserId, Guid? publicUserId, string? userName, List<string>? roles, bool isSuperAdmin, int? tenantId)
        {
            throw new NotImplementedException();
        }

        public void ClearContext()
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public Guid? GetCorrelationId()
        {
            throw new NotImplementedException();
        }
    }
}
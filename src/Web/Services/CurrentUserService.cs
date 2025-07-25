using System.Security.Claims;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace ConnectFlow.Web.Services;

public class CurrentUserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantSettings _tenantSettings;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IOptions<TenantSettings> tenantSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantSettings = tenantSettings.Value;
    }

    public int? ApplicationUserId
    {
        get
        {
            string? Id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid);

            if (int.TryParse(Id, out int applicationUserId))
            {
                return applicationUserId;
            }

            return null;
        }
    }

    public Guid? PublicUserId
    {
        get
        {
            string? Id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(Id, out Guid publicUserId))
            {
                return publicUserId;
            }

            return null;
        }
    }

    public string? UserName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        }
    }

    public IList<string> Roles
    {
        get
        {
            string? Roles = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

            if (!string.IsNullOrEmpty(Roles))
            {
                return Roles.Split(",");
            }
            else
            {
                return [];
            }
        }
    }
}
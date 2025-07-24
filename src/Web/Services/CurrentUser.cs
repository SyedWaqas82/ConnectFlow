using System.Security.Claims;

namespace ConnectFlow.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? ApplicationUserId
    {
        get
        {
            string? Id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid);

            if (!string.IsNullOrEmpty(Id))
            {
                return int.Parse(Id);
            }
            else
            {
                return null;
            }
        }
    }

    public Guid? PublicUserId
    {
        get
        {
            string? Id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(Id))
            {
                return Guid.Parse(Id);
            }
            else
            {
                return null;
            }
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
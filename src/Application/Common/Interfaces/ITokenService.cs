using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}

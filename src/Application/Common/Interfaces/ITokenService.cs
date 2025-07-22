
using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(TenantUser user);
}

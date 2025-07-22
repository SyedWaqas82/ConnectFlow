using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IRoleService
{
    Task<bool> CreateRoleAsync(string roleName, string description, string[] permissions);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<bool> UpdateRoleAsync(string roleId, string roleName, string description, string[] permissions);
    Task<bool> AssignRoleToUserAsync(string userId, string roleId);
    Task<bool> RemoveRoleFromUserAsync(string userId, string roleId);
    Task<TenantUserRole?> GetRoleByIdAsync(string roleId);
    Task<IEnumerable<TenantUserRole>> GetAllRolesAsync();
    Task<IEnumerable<TenantUserRole>> GetUserRolesAsync(string userId);
}

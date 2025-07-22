using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IRoleService
{
    Task<bool> CreateRoleAsync(string roleName, string description, string[] permissions);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<bool> UpdateRoleAsync(string roleId, string roleName, string description, string[] permissions);
    Task<bool> AssignRoleToUserAsync(string userId, string roleId);
    Task<bool> RemoveRoleFromUserAsync(string userId, string roleId);
    Task<ApplicationRole?> GetRoleByIdAsync(string roleId);
    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetUserRolesAsync(string userId);
}

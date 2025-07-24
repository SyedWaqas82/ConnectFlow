using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Identity;

public static class UserManagerExtensions
{
    public static async Task<ApplicationUser?> FindByPublicIdAsync(this UserManager<ApplicationUser> um, Guid publicId)
    {
        return await um.Users.SingleOrDefaultAsync(x => x.PublicId == publicId);
    }
}
using ConnectFlow.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public static Result<T> ToApplicationResult<T>(this IdentityResult result, T? data)
    {
        return result.Succeeded ? Result<T>.Success(data) : Result<T>.Failure(result.Errors.Select(e => e.Description), data);
    }
}

public static class UserManagerExtensions
{
    public static async Task<ApplicationUser?> FindByPublicIdAsync(this UserManager<ApplicationUser> um, Guid publicId)
    {
        return await um.Users.SingleOrDefaultAsync(x => x.PublicId == publicId);
    }
}
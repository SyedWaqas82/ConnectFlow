using ConnectFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Users.Queries.GetCurrentUserInformation;

public class GetCurrentUserInformationQuery : IRequest<UserInformationDto>;

public class GetCurrentUserInformationQueryHandler : IRequestHandler<GetCurrentUserInformationQuery, UserInformationDto>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCurrentUserInformationQueryHandler> _logger;

    public GetCurrentUserInformationQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService, IApplicationDbContext dbContext, IMapper mapper, ILogger<GetCurrentUserInformationQueryHandler> logger)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserInformationDto> Handle(GetCurrentUserInformationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user information");

        var userId = _currentUserService.GetCurrentUserId();
        var applicationUserId = _currentUserService.GetCurrentApplicationUserId();
        Guard.Against.Null(userId, message: "unknown user");

        var userResult = await _identityService.GetUserAsync(userId.Value);
        if (userResult.Succeeded == false)
        {
            _logger.LogWarning("User not found for ID {UserId}", userId);
            throw new Exception($"User not found.");
        }

        var user = userResult.Data;

        return new UserInformationDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Tenants = await _dbContext.Tenants.AsNoTracking()
                .Where(t => t.TenantUsers.Any(u => u.UserId == applicationUserId && u.IsActive)).ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken)
        };
    }
}
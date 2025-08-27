using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Users.Queries.GetCurrentUserInformation;

[Authorize]
public record GetCurrentUserInformationQuery : IRequest<UserInformationDto>;

public class GetCurrentUserInformationQueryHandler : IRequestHandler<GetCurrentUserInformationQuery, UserInformationDto>
{
    private readonly IIdentityService _identityService;
    private readonly IContextManager _contextManager;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCurrentUserInformationQueryHandler> _logger;

    public GetCurrentUserInformationQueryHandler(IIdentityService identityService, IContextManager contextManager, IApplicationDbContext dbContext, IMapper mapper, ILogger<GetCurrentUserInformationQueryHandler> logger)
    {
        _identityService = identityService;
        _contextManager = contextManager;
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserInformationDto> Handle(GetCurrentUserInformationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user information");

        var applicationUserPublicId = _contextManager.GetCurrentApplicationUserPublicId();
        var applicationUserId = _contextManager.GetCurrentApplicationUserId();
        Guard.Against.Null(applicationUserPublicId, message: "unknown user");

        var userResult = await _identityService.GetUserAsync(applicationUserPublicId.Value);
        if (userResult.Succeeded == false)
        {
            _logger.LogWarning("User not found for Public ID {ApplicationUserPublicId}", applicationUserPublicId);
            throw new Exception($"User not found.");
        }

        var user = userResult.Data;

        return new UserInformationDto
        {
            ApplicationUserPublicId = user.ApplicationUserPublicId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Tenants = await _dbContext.Tenants.AsNoTracking()
                .Where(t => t.TenantUsers.Any(u => u.ApplicationUserId == applicationUserId && u.Status == TenantUserStatus.Active)).ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken)
        };
    }
}
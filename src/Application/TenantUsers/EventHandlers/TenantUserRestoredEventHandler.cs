using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Mediator.TenantUsers;

namespace ConnectFlow.Application.TenantUsers.EventHandlers;

public class TenantUserRestoredEventHandler : INotificationHandler<TenantUserRestoredEvent>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<TenantUserRestoredEventHandler> _logger;

    public TenantUserRestoredEventHandler(IApplicationDbContext dbContext, IIdentityService identityService, IMessagePublisher messagePublisher, ILogger<TenantUserRestoredEventHandler> logger)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(TenantUserRestoredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConnectFlow Domain Event: {DomainEvent} for TenantUserId: {TenantUserId}", notification.GetType().Name, notification.TenantUser.Id);

        try
        {
            // Get the tenant user information, ignoring query filters to get even suspended users
            var tenantUser = notification.TenantUser;

            // Get the user information
            var userResult = await _identityService.GetUserAsync(tenantUser.ApplicationUserId);
            if (!userResult.Succeeded)
            {
                _logger.LogWarning("Failed to get user information for ApplicationUserId: {ApplicationUserId}", tenantUser.ApplicationUserId);
                return;
            }

            var userData = userResult.Data;

            //check if subscription does not have tenant loaded then load now
            if (tenantUser.Tenant == null)
            {
                tenantUser.Tenant = await _dbContext.Tenants.FindAsync(new object[] { tenantUser.TenantId }, cancellationToken) ?? throw new TenantNotFoundException($"Tenant not found for user {tenantUser.Id}");
            }

            var emailEvent = new EmailSendMessageEvent(tenantUser.TenantId, tenantUser.ApplicationUserId)
            {
                CorrelationId = notification.CorrelationId,
                ApplicationUserPublicId = notification.ApplicationUserPublicId,
                To = userData.Email,
                Subject = "Account Access Restored - Welcome Back!",
                IsHtml = true,
                TemplateId = EmailTemplates.TenantUserRestored,
                TemplateData = new Dictionary<string, object>
                {
                    { "TenantName", tenantUser.Tenant.Name },
                    { "UserName", $"{userData.FirstName} {userData.LastName}" },
                    { "FirstName", userData.FirstName },
                    { "LastName", userData.LastName },
                    { "Email", userData.Email },
                    { "RestorationReason", "Subscription plan updated and limits restored" },
                    { "SupportEmail", "support@connectflow.com" },
                    { "LoginUrl", "https://app.connectflow.com/login" },
                    { "TenantId", tenantUser.TenantId },
                    { "ApplicationUserId", tenantUser.ApplicationUserId },
                    { "ApplicationUserPublicId", notification.ApplicationUserPublicId.GetValueOrDefault() },
                    { "CorrelationId", notification.CorrelationId.GetValueOrDefault() },
                    { "RestoredAt", DateTimeOffset.UtcNow }
                },
            };

            var queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);
            await _messagePublisher.PublishAsync(emailEvent, queue.RoutingKey, cancellationToken);

            _logger.LogInformation("Tenant user restoration email queued for user {Email} in tenant {TenantName}", userData.Email, tenantUser.Tenant.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process TenantUserRestoredEvent for TenantUserId: {TenantUserId}", notification.TenantUser.Id);
            throw;
        }
    }
}
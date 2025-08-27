# Context Management in ConnectFlow

This document explains how to use the context management system in ConnectFlow, which provides access to user and tenant information throughout the application.

## Overview

ConnectFlow uses a unified context management system that works seamlessly in both HTTP contexts (API requests) and non-HTTP contexts (background jobs, message consumers). The system consists of:

- **Scoped Context Service**: DI-scoped context storage for all environments
- **Context Interfaces**: Clean separation of concerns with interface segregation
- **Middleware**: Automatic context setup for HTTP requests
- **Context Initialization Methods**: Explicit context setup for non-HTTP scenarios
- **Context Validation**: Utilities to ensure context is properly set

## Key Components

### Interfaces

- **ICurrentUserService**: Access user identity information
- **ICurrentTenantService**: Access tenant information
- **IContextManager**: Initialize and manage context state

### Implementation

All interfaces are implemented by the `UnifiedContextService` class, providing a cohesive approach to context management.

## Setting Context

### In HTTP Context (Web API)

HTTP context is automatically initialized by the `ContextMiddleware` which:

1. Extracts user identity from authentication claims
2. Extracts tenant information from headers or claims
3. Makes this information available throughout the request lifecycle

The middleware is registered in the application pipeline:

```csharp
app.UseMiddleware<ContextMiddleware>();
```

**HTTP Headers for Context**:

- `X-Tenant-ID`: Sets the current tenant ID

### In Non-HTTP Context (Background Jobs, Services)

For non-HTTP contexts (background jobs, services, etc.), explicitly initialize the context using `IContextManager`:

```csharp
// Option 1: Initialize with specific user ID and tenant ID
await _contextManager.InitializeContextAsync(applicationUserId, tenantId);

// Option 2: Initialize with user ID and default tenant
await _contextManager.InitializeContextWithDefaultTenantAsync(applicationUserId);

// Option 3: Manually set specific context values
_contextManager.SetContext(
    applicationUserId: 123, 
    applicationUserPublicId: Guid.Parse("..."), 
    userName: "username", 
    roles: new List<string> { "Admin" }, 
    isSuperAdmin: false, 
    tenantId: 456
);
```

**Always clear the context when done**:

```csharp
_contextManager.ClearContext();
```

Or use the async version:

```csharp
await _contextManager.ClearContextAsync();
```

### In Message Handlers and RabbitMQ Consumers

When working with message handlers (like RabbitMQ consumers), the context should be initialized in the consumer scope and verified in the handler:

```csharp
// In RabbitMQConsumerService.HandleMessageAsync:
using var scope = _serviceProvider.CreateScope();
var contextManager = scope.ServiceProvider.GetRequiredService<IContextManager>();
var handler = scope.ServiceProvider.GetService<IMessageHandler<T>>();

// Initialize context from message
await contextManager.InitializeContextAsync(message.ApplicationUserId.GetValueOrDefault(), message.TenantId);

// Log to verify context was set correctly
_logger.LogDebug("Context initialized: ApplicationUserId={ApplicationUserId}, TenantId={TenantId}", 
    message.ApplicationUserId, message.TenantId);

// Call handler (which should use the same scope)
await handler.HandleAsync(message, cancellationToken);
```

Then, in your handler, verify the context is available and restore it if necessary:

```csharp
public async Task HandleAsync(TMessage message, CancellationToken cancellationToken)
{
    // Check if context is available
    var applicationUserId = _currentUserService.GetCurrentApplicationUserId();
    var tenantId = _currentTenantService.GetCurrentTenantId();
    
    // If context is missing, restore it from the message
    if (applicationUserId == null || tenantId == null)
    {
        _logger.LogWarning("Context not flowing to handler, restoring from message values");
        
        if (_currentUserService is IContextManager contextManager)
        {
            await contextManager.InitializeContextAsync(
                message.ApplicationUserId ?? 0,
                message.TenantId);
                
            // Verify context was restored
            _logger.LogInformation("Context restored: ApplicationUserId={ApplicationUserId}, TenantId={TenantId}",
                _currentUserService.GetCurrentApplicationUserId(),
                _currentTenantService.GetCurrentTenantId());
        }
    }
    
    // Continue processing with context available
}
```

## Accessing Context

### User Information

```csharp
// Through dependency injection
public class MyService 
{
    private readonly ICurrentUserService _currentUserService;

    public MyService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public void DoSomething()
    {
        var appicationPublicUserId = _currentUserService.GetCurrentApplicationUserId();
        var applicationUserId = _currentUserService.GetCurrentApplicationUserId();
        var username = _currentUserService.GetCurrentUserName();
        var roles = _currentUserService.GetCurrentUserRoles();
        var isSuperAdmin = _currentUserService.IsSuperAdmin();
    }
}
```

### Tenant Information

```csharp
// Through dependency injection
public class MyService 
{
    private readonly ICurrentTenantService _tenantService;

    public MyService(ICurrentTenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public void DoSomething()
    {
        var tenantId = _tenantService.GetCurrentTenantId();
    }
}
```

## Best Practices

### Always Use Dependency Injection

Always access context through injected interfaces:

```csharp
// Good
public class MyService(ICurrentUserService currentUserService, ICurrentTenantService tenantService)
{
    // Use currentUserService and tenantService
}
```

### Verify Context in Message Handlers

Always check if context is available in message handlers and restore it if necessary:

```csharp
// In message handler
public async Task HandleAsync(TMessage message, CancellationToken cancellationToken)
{
    // Check if context is available
    var applicationUserId = _currentUserService.GetCurrentApplicationUserId();
    var tenantId = _currentTenantService.GetCurrentTenantId();
    
    if (applicationUserId == null || tenantId == null)
    {
        _logger.LogWarning("Context not flowing to handler, restoring from message values");
        if (_currentUserService is IContextManager contextManager)
        {
            await contextManager.InitializeContextAsync(
                message.ApplicationUserId ?? 0,
                message.TenantId);
        }
    }
    
    // Now proceed with guaranteed context
}
```

### Clear Context When Done

Always clear context when finishing background operations to prevent leaking context to other operations:

```csharp
try
{
    await _contextManager.InitializeContextAsync(applicationUserId, tenantId);
    // Do work with context
}
finally
{
    await _contextManager.ClearContextAsync();
}
```

### Multi-tenant Awareness

Always check tenant context in multi-tenant operations:

```csharp
public async Task<Result> DoSomethingWithData()
{
    var tenantId = _tenantService.GetCurrentTenantId();
    if (!tenantId.HasValue)
    {
        return Result.Failure("No tenant context available");
    }

    // Proceed with tenant-aware operation
}
```

## Implementation Details

### Scoped Context Management

Context information is stored as instance fields in the `UnifiedContextService` class:

```csharp
public class UnifiedContextService : IContextManager
{
    private int? _currentTenantId { get; set; } = null;
    private int? _applicationUserId { get; set; } = null;
    private Guid? _publicUserId { get; set; } = null;
    private string? _userName { get; set; } = string.Empty;
    private List<string> _roles { get; set; } = new List<string>();
    private bool _isSuperAdmin { get; set; } = false;
    
    // ... methods and other members
}
```

This approach ensures that:

1. Context is properly scoped to the current DI scope/request
2. Multiple concurrent requests get their own context instance
3. Context is consistent within a single scope

### Context in Message Processing

When working with message handlers (such as RabbitMQ consumers), keep these considerations in mind:

1. Each consumer creates its own DI scope for processing a message
2. Context must be initialized within that scope using `InitializeContextAsync`
3. Services within that scope (handlers, repositories, etc.) will see the correct context
4. Different messages/consumers get their own context instances

To ensure reliable context flow:

- Always use scoped lifetime for context-related services
- Create proper DI scopes for background operations
- Verify context is available in handlers and restore if needed
- For critical operations, consider passing context explicitly in method parameters

### Context Validation

Use `IContextValidationService` for enforcing context rules:

```csharp
public class MyService
{
    private readonly IContextValidationService _contextValidationService;

    public MyService(IContextValidationService contextValidationService)
    {
        _contextValidationService = contextValidationService;
    }

    public async Task DoSomething()
    {
        // Ensures tenant context is set and user has access to it
        await _contextValidationService.ValidateTenantContextAsync();
        
        // Proceed with operation
    }
}
```

## Debugging Context Issues

If you encounter context-related issues:

1. Check if context is properly initialized
2. Verify HTTP headers for tenant information
3. Ensure context is cleared after background operations
4. Review logs with the `ConnectFlow.Infrastructure.Services` category

### Common Context Issues and Solutions

#### Context Not Available in Handler

**Problem**: Context is set in the consumer but not accessible in the handler.  
**Solution**: Ensure both consumer and handler use the same DI scope, and that context is initialized before calling the handler.

#### Context Inconsistency Between Services

**Problem**: Different services in the same operation see different context values.  
**Solution**: Ensure all services are resolved from the same scope, and that all context interfaces resolve to the same instance of `UnifiedContextService`.

#### Context Missing After Scope Creation

**Problem**: Context is lost when creating a new scope.  
**Solution**: Explicitly initialize context in any new scope, either by passing values from parent scope or from message data.

## Example Scenarios

### Background Job

```csharp
public class EmailJob
{
    private readonly IContextManager _contextManager;
    private readonly IEmailService _emailService;

    public EmailJob(IContextManager contextManager, IEmailService emailService)
    {
        _contextManager = contextManager;
        _emailService = emailService;
    }

    public async Task SendEmailAsync(int applicationUserId, int tenantId, string emailContent)
    {
        try
        {
            // Initialize context for background processing
            await _contextManager.InitializeContextAsync(applicationUserId, tenantId);
            
            // Use context in email service
            await _emailService.SendEmailAsync(emailContent);
        }
        finally
        {
            // Always clear context when done
            await _contextManager.ClearContextAsync();
        }
    }
}
```

### Message Handler with Context Verification

```csharp
public class EmailSendMessageEventHandler : IMessageHandler<EmailSendMessageEvent>
{
    private readonly IEmailService _emailService;
    private readonly ICurrentTenantService _currentTenantService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<EmailSendMessageEventHandler> _logger;
    
    public EmailSendMessageEventHandler(
        IEmailService emailService, 
        ICurrentTenantService currentTenantService, 
        ICurrentUserService currentUserService, 
        ILogger<EmailSendMessageEventHandler> logger)
    {
        _emailService = emailService;
        _currentTenantService = currentTenantService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task HandleAsync(EmailSendMessageEvent message, CancellationToken cancellationToken)
    {
        try
        {
            // Log and verify context values
            var applicationUserId = _currentUserService.GetCurrentApplicationUserId();
            var tenantId = _currentTenantService.GetCurrentTenantId();
            _logger.LogDebug("Context values - Service: [ApplicationUserId: {ServiceUserId}, TenantId: {ServiceTenantId}], Message: [ApplicationUserId: {MessageUserId}, TenantId: {MessageTenantId}]", 
                applicationUserId, tenantId, message.ApplicationUserId, message.TenantId);
            
            // If context is missing, restore it from message
            if (applicationUserId == null || tenantId == null)
            {
                _logger.LogWarning("Context not flowing to handler, manually setting from message values");
                
                if (_currentUserService is IContextManager contextManager)
                {
                    _logger.LogInformation("Setting context directly: ApplicationUserId={ApplicationUserId}, TenantId={TenantId}", 
                        message.ApplicationUserId, message.TenantId);
                        
                    await contextManager.InitializeContextAsync(
                        message.ApplicationUserId ?? 0,
                        message.TenantId);
                }
            }

            // Now proceed with context available
            var email = new EmailMessage
            {
                To = message.To,
                Subject = message.Subject,
                // ...other properties
            };
            
            await _emailService.SendAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process email message");
            throw;
        }
    }
}
```

### API Controller with Custom Tenant

```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomTenantController : ControllerBase
{
    private readonly ICurrentTenantService _tenantService;
    private readonly IMyService _myService;

    public CustomTenantController(
        ICurrentTenantService tenantService,
        IMyService myService)
    {
        _tenantService = tenantService;
        _myService = myService;
    }

    [HttpGet("data/{tenantId}")]
    public async Task<ActionResult<DataDto>> GetDataForTenant(int tenantId)
    {
        // Store original tenant ID
        var originalTenantId = _tenantService.GetCurrentTenantId();
        
        try
        {
            // Set custom tenant ID for this operation
            ((UnifiedContextService)_tenantService).SetCurrentTenantId(tenantId);
            
            // Proceed with operation using the custom tenant context
            var result = await _myService.GetDataAsync();
            return Ok(result);
        }
        finally
        {
            // Restore original tenant ID
            ((UnifiedContextService)_tenantService).SetCurrentTenantId(originalTenantId);
        }
    }
}
```

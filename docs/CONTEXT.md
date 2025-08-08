# Context Management in ConnectFlow

This document explains how to use the context management system in ConnectFlow, which provides access to user and tenant information throughout the application.

## Overview

ConnectFlow uses a unified context management system that works seamlessly in both HTTP contexts (API requests) and non-HTTP contexts (background jobs, console applications). The system consists of:

- **Static Context Classes**: Thread-safe storage using `AsyncLocal<T>`
- **Context Interfaces**: Clean separation of concerns with interface segregation
- **Middleware**: Automatic context setup for HTTP requests
- **Context Initialization Methods**: Explicit context setup for non-HTTP scenarios

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
    publicUserId: Guid.Parse("..."), 
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

## Accessing Context

### User Information

```csharp
// Through dependency injection (recommended)
public class MyService 
{
    private readonly ICurrentUserService _currentUserService;

    public MyService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public void DoSomething()
    {
        var userId = _currentUserService.GetCurrentUserId();
        var appUserId = _currentUserService.GetCurrentApplicationUserId();
        var username = _currentUserService.GetCurrentUserName();
        var roles = _currentUserService.GetCurrentUserRoles();
        var isSuperAdmin = _currentUserService.IsSuperAdmin();
    }
}

// Directly via static class (use cautiously)
var userId = UserInfo.PublicUserId;
var appUserId = UserInfo.ApplicationUserId;
var username = UserInfo.UserName;
var roles = UserInfo.Roles;
var isSuperAdmin = UserInfo.IsSuperAdmin;
```

### Tenant Information

```csharp
// Through dependency injection (recommended)
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

// Directly via static class (use cautiously)
var tenantId = TenantInfo.CurrentTenantId;
```

## Best Practices

### Always Use Dependency Injection

Prefer accessing context through injected interfaces rather than static classes:

```csharp
// Good
public class MyService(ICurrentUserService currentUserService, ICurrentTenantService tenantService)
{
    // Use currentUserService and tenantService
}

// Avoid when possible
public class MyService
{
    public void DoSomething()
    {
        var userId = UserInfo.PublicUserId;
        var tenantId = TenantInfo.CurrentTenantId;
    }
}
```

### Clear Context When Done

Always clear context when finishing background operations to prevent leaking context to other operations:

```csharp
try
{
    await _contextManager.InitializeContextAsync(userId, tenantId);
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

### Thread Safety

All context information is stored using `AsyncLocal<T>` to ensure thread safety across asynchronous operations:

```csharp
private static readonly AsyncLocal<int?> _applicationUserId = new AsyncLocal<int?>();
private static readonly AsyncLocal<int?> _currentTenantId = new AsyncLocal<int?>();
```

This approach ensures that context follows the logical execution flow rather than being tied to a specific thread.

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

    public async Task SendEmailAsync(int userId, int tenantId, string emailContent)
    {
        try
        {
            // Initialize context for background processing
            await _contextManager.InitializeContextAsync(userId, tenantId);
            
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

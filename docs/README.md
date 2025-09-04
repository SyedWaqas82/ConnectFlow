# ConnectFlow Development Guide

ConnectFlow is a modern, multi-tenant SaaS platform for service businesses, built with Clean Architecture principles and enterprise-grade patterns.

## Architecture Overview

ConnectFlow implements Clean Architecture with these key layers:

- **Domain Layer** - Core business entities, value objects, enumerations, and domain events
- **Application Layer** - Use cases (commands/queries), business logic, event handlers, and application services
- **Infrastructure Layer** - External service integrations, data persistence, messaging, and cross-cutting concerns
- **Web Layer** - HTTP endpoints, middleware, authentication, and API presentation

### Key Features

- **Multi-tenant SaaS Architecture** with complete tenant isolation and context management
- **Subscription & Payment Processing** with Stripe integration and webhook processing
- **User Management** with JWT authentication, refresh tokens, and role-based access control
- **Enhanced Background Job Processing** using Quartz.NET with automatic metrics, correlation tracking, and distributed tracing
- **Domain-Driven Asynchronous Messaging** with RabbitMQ for reliable event processing with retry and dead letter patterns
- **Comprehensive Observability Stack** with OpenTelemetry, Prometheus, Loki, Tempo, and Grafana dashboards
- **Unified Context Management** for seamless user/tenant context across HTTP and background operations
- **Rate Limiting** with token bucket and fixed window algorithms for API protection
- **Health Monitoring** with comprehensive health checks and dedicated monitoring UI

## Application Structure

### Core Entities

The application is built around these primary domain entities:

- **Tenant** - Multi-tenant organization structure with subscriptions and billing
- **User** - Application users with authentication and profile management
- **Subscription** - Billing and plan management with Stripe integration
- **Plan** - Service tiers and pricing configuration

### Application Layers

#### Domain Layer (`src/Domain`)
- **Entities** - Core business objects (Tenant, User, Subscription, Plan, etc.)
- **Value Objects** - Immutable objects representing domain concepts
- **Enumerations** - Domain-specific enums and constants
- **Events** - Domain events for decoupled communication
- **Exceptions** - Domain-specific error types

#### Application Layer (`src/Application`)
- **Users** - Authentication, registration, password management
- **Subscriptions** - Plan management, billing, payment processing
- **Common** - Shared behaviors, interfaces, models, and messaging
- **WeatherForecasts** - Example CQRS implementation (demo endpoint)

#### Infrastructure Layer (`src/Infrastructure`)
- **Data** - Entity Framework configurations and database context
- **Identity** - ASP.NET Core Identity implementation
- **Services** - External service integrations (email, payment, messaging)
- **Messaging** - RabbitMQ implementation for asynchronous communication
- **Metrics** - Application metrics and monitoring infrastructure

#### Web Layer (`src/Web`)
- **Endpoints** - HTTP API endpoints grouped by feature
- **Middleware** - Request/response processing and cross-cutting concerns
- **Infrastructure** - Web-specific configuration and extensions

## Build and Development

### Prerequisites

- .NET 9 SDK
- PostgreSQL (primary database)
- Redis (caching and session storage)
- RabbitMQ (message queuing)
- Stripe Account (payment processing)

### Local Development Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd ConnectFlow
   ```

2. **Start infrastructure services**

   ```bash
   docker-compose up -d postgres redis rabbitmq grafana prometheus loki tempo mailhog
   ```

3. **Configure application settings**
   
   Update `src/Web/appsettings.Development.json` with your local configuration:
   
   ```json
   {
     "ConnectionStrings": {
       "ConnectFlowDb": "Host=localhost;Database=ConnectFlow_Dev;Username=postgres;Password=postgres",
       "Redis": "localhost:6379"
     },
     "RabbitMQ": {
       "HostName": "localhost",
       "Port": 5672,
       "UserName": "guest",
       "Password": "guest"
     },
     "StripeSettings": {
       "SecretKey": "sk_test_...",
       "WebhookSecret": "whsec_..."
     },
     "Monitoring": {
       "UseLoki": true,
       "LokiUrl": "http://localhost:3100",
       "OtlpEndpoint": "http://localhost:4317"
     }
   }
   ```

4. **Initialize the database**

   ```bash
   cd src/Infrastructure
   dotnet ef database update
   ```

5. **Build the solution**

   ```bash
   dotnet build -tl
   ```

6. **Run the application**

   ```bash
   cd src/Web
   dotnet watch run
   ```

   Navigate to <https://localhost:5001> for the API.

### Development Access URLs

- **Application API**: <https://localhost:5001>
- **Grafana Dashboards**: <http://localhost:3000> (admin/admin)
- **RabbitMQ Management**: <http://localhost:15672> (guest/guest)
- **MailHog Email UI**: <http://localhost:8025>
- **Prometheus Metrics**: <http://localhost:9090>

### Code Scaffolding

The solution supports scaffolding new CQRS features using the Clean Architecture template:

**Create a new command:**

```bash
cd src/Application
dotnet new ca-usecase --name CreateExample --feature-name Examples --usecase-type command --return-type int
```

**Create a new query:**

```bash
cd src/Application
dotnet new ca-usecase -n GetExamples -fn Examples -ut query -rt ExamplesVm
```

### Development Tools

**Code Styles & Formatting:**
The project includes EditorConfig support for consistent code formatting across development environments.

**API Versioning:**
The solution supports API versioning using `Asp.Versioning` package. See `WeatherForecasts` endpoint for version implementation examples.

## Testing

The solution includes comprehensive testing at multiple levels:

### Test Projects

- **Application.UnitTests** - Application layer unit tests
- **Domain.UnitTests** - Domain layer unit tests  
- **Infrastructure.IntegrationTests** - Infrastructure integration tests
- **Application.FunctionalTests** - End-to-end functional tests

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Application.UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Database

The functional tests use Testcontainers for integration testing with real database instances, ensuring tests run against the same database engine used in production.

## Configuration

### Environment-Specific Settings

The application uses the standard .NET configuration system with support for:

- **appsettings.json** - Base configuration
- **appsettings.Development.json** - Development overrides
- **appsettings.Production.json** - Production configuration
- **Environment Variables** - Runtime configuration overrides
- **Azure Key Vault** - Secure configuration storage (production)

### Key Configuration Sections

- **ConnectionStrings** - Database and Redis connections
- **JwtSettings** - JWT token configuration with refresh token support
- **RabbitMQ** - Message queue settings with connection management
- **StripeSettings** - Payment processing configuration with webhook secrets
- **SubscriptionSettings** - Billing and subscription behavior configuration
- **EmailSettings** - SMTP and email template configuration
- **Monitoring** - Observability stack configuration (Loki, Tempo, Prometheus)
- **RateLimiting** - API protection with token bucket and fixed window algorithms
- **QuartzSettings** - Background job processing configuration with clustering support

## Development Guidelines

### Clean Architecture Principles

1. **Dependency Rule** - Dependencies point inward toward the domain
2. **Interface Segregation** - Use focused interfaces for specific capabilities
3. **Single Responsibility** - Each class has one reason to change
4. **Domain-Driven Design** - Model real business concepts and workflows

### CQRS Implementation

ConnectFlow implements the CQRS (Command Query Responsibility Segregation) pattern:

- **Commands** - Modify state, return results or void
- **Queries** - Read data, never modify state
- **Handlers** - Process commands and queries using MediatR
- **Validators** - Validate requests using FluentValidation
- **Authorization** - Role-based and resource-based authorization

### Domain Events

The application uses domain events for decoupled communication:

- **Domain Events** - In-process notifications using MediatR
- **Message Events** - Cross-service communication via RabbitMQ
- **Integration Events** - External system integration patterns

## Deployment

### Docker Support

The application includes Docker support with multi-stage builds:

```bash
# Build image
docker build -t connectflow .

# Run with docker-compose
docker-compose up -d
```

### Production Considerations

- **Database Migrations** - Use `dotnet ef database update` in deployment pipeline
- **Configuration** - Use Azure Key Vault or environment variables for secrets
- **Monitoring** - Configure OpenTelemetry exports to your monitoring system
- **Health Checks** - Monitor `/health` endpoint for application status

## Additional Resources

For detailed information about specific subsystems, see the following documentation:

- **[Context Management](./CONTEXT.md)** - User and tenant context handling
- **[Background Jobs](./QUARTZ.md)** - Job scheduling and processing
- **[Messaging](./RABBITMQ.md)** - RabbitMQ integration and message handling
- **[Payment System](./PAYMENT_SUBSCRIPTION.md)** - Stripe integration and subscription management
- **[Monitoring](./MONITORING.md)** - Observability, metrics, and health checks

### External Resources

- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture) - Original template documentation
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/) - Official .NET documentation
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Data access documentation

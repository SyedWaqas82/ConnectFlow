# ConnectFlow

ConnectFlow is a modern, multi-tenant SaaS platform for service businesses, built with .NET 9 using Clean Architecture principles. It provides comprehensive customer relationship management (CRM), subscription billing, appointment scheduling, and AI-powered customer service capabilities.

## Features

- **Multi-tenant Architecture** - Complete tenant isolation and management with context propagation
- **Subscription Management** - Stripe-integrated billing with webhook processing and payment retry logic
- **User Management** - JWT-based authentication with role-based access control and refresh tokens
- **Context Management** - Unified context system working seamlessly in HTTP and background operations
- **Background Jobs** - Enhanced Quartz.NET framework with automatic metrics, correlation tracking, and distributed tracing
- **Messaging System** - RabbitMQ-powered asynchronous communication with domain-driven queue management
- **Observability Stack** - Complete MELT observability with OpenTelemetry, Prometheus, Loki, Tempo, and Grafana
- **Email System** - Template-based email processing with MailHog for development testing
- **Rate Limiting** - Token bucket and fixed window rate limiting for API protection
- **Health Monitoring** - Comprehensive health checks with dedicated monitoring UI

## Documentation

### Core System Documentation

- **[Setup & Development Guide](./docs/README.md)** - Development environment setup and basic usage
- **[Context Management](./docs/CONTEXT.md)** - User and tenant context handling across the application
- **[Background Jobs (Quartz)](./docs/QUARTZ.md)** - Enhanced job framework with metrics, correlation tracking, and distributed tracing
- **[Messaging (RabbitMQ)](./docs/RABBITMQ.md)** - Domain-driven asynchronous message processing and queue management
- **[Payment & Subscriptions](./docs/PAYMENT_SUBSCRIPTION.md)** - Stripe integration, billing, subscription lifecycle, and webhook handling
- **[Monitoring & Observability](./docs/MONITORING.md)** - MELT observability stack with metrics, logs, traces, and comprehensive dashboards

### Quick Start

1. **Prerequisites**: .NET 9 SDK, Docker & Docker Compose
2. **Clone and Setup**: `git clone <repo> && cd ConnectFlow`
3. **Start Services**: `docker-compose up -d postgres redis rabbitmq grafana prometheus loki tempo`
4. **Configuration**: Update `appsettings.Development.json` with your configuration
5. **Database**: `dotnet ef database update --project src/Infrastructure`
6. **Run**: `cd src/Web && dotnet run`

### Monitoring Access

- **Application**: <https://localhost:5001>
- **Grafana Dashboards**: <http://localhost:3000> (admin/admin)
- **RabbitMQ Management**: <http://localhost:15672> (guest/guest)
- **MailHog (Email Testing)**: <http://localhost:8025>
- **Prometheus**: <http://localhost:9090>

## Architecture

ConnectFlow follows Clean Architecture with these layers:

- **Domain** - Core business entities, value objects, and domain events
- **Application** - Use cases, commands, queries, and business logic
- **Infrastructure** - External services, data access, and cross-cutting concerns  
- **Web** - API endpoints, middleware, and presentation logic


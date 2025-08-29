# ConnectFlow

ConnectFlow is a modern, multi-tenant SaaS platform for service businesses, built with .NET 9 using Clean Architecture principles. It provides comprehensive customer relationship management (CRM), subscription billing, appointment scheduling, and AI-powered customer service capabilities.

## Features

- **Multi-tenant Architecture** - Complete tenant isolation and management
- **Subscription Management** - Stripe-integrated billing and payment processing
- **User Management** - JWT-based authentication with role-based access control
- **Context Management** - Unified context system for HTTP and background operations
- **Background Jobs** - Quartz.NET-based job processing with comprehensive monitoring
- **Messaging System** - RabbitMQ-powered asynchronous communication
- **Observability** - OpenTelemetry, Prometheus metrics, and Grafana dashboards
- **Email System** - Template-based email processing with retry logic

## Documentation

### Core System Documentation
- **[Setup & Development Guide](./docs/README.md)** - Development environment setup and basic usage
- **[Context Management](./docs/CONTEXT.md)** - User and tenant context handling across the application
- **[Background Jobs (Quartz)](./docs/QUARTZ.md)** - Job scheduling, processing, and monitoring
- **[Messaging (RabbitMQ)](./docs/RABBITMQ.md)** - Asynchronous message processing and queue management
- **[Payment & Subscriptions](./docs/PAYMENT_SUBSCRIPTION.md)** - Stripe integration, billing, and subscription lifecycle
- **[Monitoring & Observability](./docs/MONITORING.md)** - Metrics, logging, tracing, and health checks

### Quick Start

1. **Prerequisites**: .NET 9 SDK, PostgreSQL, Redis, RabbitMQ
2. **Clone and Setup**: `git clone <repo> && cd ConnectFlow`
3. **Configuration**: Update `appsettings.Development.json` with your database and service connections
4. **Database**: `dotnet ef database update --project src/Infrastructure`
5. **Run**: `cd src/Web && dotnet run`

## Architecture

ConnectFlow follows Clean Architecture with these layers:
- **Domain** - Core business entities, value objects, and domain events
- **Application** - Use cases, commands, queries, and business logic
- **Infrastructure** - External services, data access, and cross-cutting concerns  
- **Web** - API endpoints, middleware, and presentation logic


# System Architecture Overview

Our platform follows a Clean Architecture approach with domain-driven design principles.

## Layers
- **Domain Layer**: Contains core business rules and entities. No external dependencies.
- **Application Layer**: Contains use cases, DTOs/Commands/Queries, interfaces.
- **Infrastructure Layer**: EF Core repositories, external services, caching, messaging.
- **API Layer**: Minimal API endpoints using .NET 8.

## Patterns
- CQRS is used for separating read and write models.
- MediatR is used for handling requests between layers.
- All async methods MUST end with the suffix "Async".

## Deployment
- GitHub Actions handles CI pipeline.
- Azure App Service hosts the API and workers.
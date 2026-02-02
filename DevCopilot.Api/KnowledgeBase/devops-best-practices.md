# DevOps Best Practices

## CI/CD
- Every pull request must pass build + test steps before merging.
- Code coverage must remain above 85%.
- Release pipelines automatically deploy to staging, then require approval for production.

## Branching Strategy
- We follow GitHub Flow: feature branches → pull request → main.

## Observability
- All APIs must include structured logging.
- OpenTelemetry is enabled for distributed tracing.
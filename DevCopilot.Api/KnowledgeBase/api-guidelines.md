# API Guidelines

## Versioning
- All public APIs should be versioned using URI versioning. Example: /api/v1/users.

## Error Handling
- Use problem-details format for errors.
- Do not expose internal exceptions.

## Pagination
- Default page size is 20.
- Maximum page size is 100.

## Security
- All endpoints require JWT authentication unless marked as public.
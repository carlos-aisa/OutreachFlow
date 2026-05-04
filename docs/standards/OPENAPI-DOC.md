# OpenAPI Documentation Standard

## Purpose

This document defines how API contracts must be documented in this repository.

## Mandatory Rules

- Every contract change must update the OpenAPI document.
- The OpenAPI source of truth for v1 is:
  - `docs/api/openapi.v1.yaml`
- API routes, status codes, and schemas must match implementation.
- Documentation must be in English.

## Change Checklist

When any endpoint is added or changed:

1. Update `docs/api/openapi.v1.yaml`.
2. Verify path, method, request body, response bodies, and status codes.
3. Keep operation IDs stable when possible.
4. Ensure examples do not contain secrets or personal data.

## Versioning

- Use `/api/v1/...` route versioning for the current API surface.
- Breaking changes must be introduced in a new version.

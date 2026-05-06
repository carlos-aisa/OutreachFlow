## Context

The application currently assumes a single language experience for UI labels and many user-facing messages. Introducing Spanish localization requires coordinated changes across Blazor pages, shared UI components, request validation feedback, and API-facing error payload content while preserving existing behavior.

## Goals / Non-Goals

**Goals:**
- Provide Spanish localized UI labels, form text, and common user actions.
- Provide Spanish localized user-facing validation and error messages.
- Configure culture resolution and default language behavior for Spanish users.
- Keep localization extensible for future additional languages.

**Non-Goals:**
- Machine translation of user-generated content.
- Region-specific dialect customization in this phase.
- Full multilingual OpenAPI document duplication.

## Decisions

- Use ASP.NET Core localization primitives and resource files to avoid introducing new framework dependencies.
- Add centralized resource keys for shared Web UI text and per-feature resource groups for maintainability.
- Resolve culture from user preference setting and Accept-Language header fallback, with deterministic default culture.
- Localize API validation/error messages by mapping known application error codes to resource-backed text.
- Keep domain logic language-agnostic; only presentation and application boundary messages are localized.

## Risks / Trade-offs

- Partial translation can produce mixed-language UX -> Mitigation: define translation coverage baseline and add tests for critical screens and error paths.
- Localization key sprawl can hurt maintainability -> Mitigation: enforce feature-based resource organization and key naming convention.
- Culture negotiation differences between browsers can cause inconsistent language -> Mitigation: prioritize explicit user selection over header-derived values.

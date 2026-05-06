# Localization Guide

OutreachFlow currently supports two UI/API cultures:

- `en-US` (default fallback)
- `es-ES`

## Culture Resolution Order

Both `OutreachFlow.Web` and `OutreachFlow.Api` resolve culture in this order:

1. Explicit query string (`?culture=...` and optional `ui-culture`)
2. Localization cookie
3. `Accept-Language` request header
4. Configured default (`en-US`)

This keeps behavior deterministic while allowing explicit user override.

## Web Language Selection

The sidebar includes a language selector (`English` / `Español`).

Selecting a language calls:

- `GET /culture/set?culture=<culture>&redirectUri=<relative-path>`

The endpoint stores `CookieRequestCultureProvider.DefaultCookieName` and redirects back to the requested page.

## Resource File Conventions

Web resources:

- `src/OutreachFlow.Web/Resources/SharedResource.resx`
- `src/OutreachFlow.Web/Resources/SharedResource.es.resx`

API user-facing error resources:

- `src/OutreachFlow.Api/Resources/ApiErrorResource.resx`
- `src/OutreachFlow.Api/Resources/ApiErrorResource.es.resx`

### Key Naming

- Navigation keys: `Nav.*`
- Shared labels/actions: `Common.*`
- Feature/page keys: `<Feature>.*` (for example `Contacts.*`, `Templates.*`)
- API error keys: `ApiError.*`

## Localized API Errors

`ApiEndpoint` localizes known error messages through `IApiErrorLocalizer` before returning `ApiError`.

Unknown messages are returned as-is to preserve behavior and avoid masking unexpected states.

## How To Add A New Translation Key

1. Add the key to `SharedResource.resx` (English baseline).
2. Add the same key to `SharedResource.es.resx` (Spanish translation).
3. Reference the key via `IStringLocalizer<SharedResource>` in the Razor component.
4. Add or update tests to validate Spanish rendering for changed flows.

For API errors:

1. Add the English/Spanish key pair in `ApiErrorResource*.resx`.
2. Map the known message in `ApiErrorLocalizer.MessageKeyMap`.
3. Add integration coverage for localized response behavior if user-visible.

## Context

Templates and sender profiles exist after Phase 2. The renderer must remain simple and predictable so outreach remains human-controlled and testable.

## Goals / Non-Goals

**Goals:**

- Render subject and body templates using supported variables.
- Detect unknown variables.
- Detect supported variables that have no value in the current context.
- Return structured render diagnostics without throwing for normal validation failures.

**Non-Goals:**

- Loops, conditionals, formatting expressions, or scripting.
- HTML sanitization or rich text editing.
- Draft persistence or sending.

## Decisions

- Implement a simple token parser for `{{variable.path}}` syntax. This avoids introducing a full template engine before it is needed.
- Keep the supported variable registry centralized and explicit. This makes UI guidance, validation, and tests consistent.
- Return `RenderedEmail` with diagnostics instead of throwing for user-authored template problems. This keeps validation visible to draft generation and review workflows.
- Treat whitespace inside token delimiters as acceptable, while variable names must exactly match the registry after trimming.

## Risks / Trade-offs

- A simple renderer will not support advanced personalization. Mitigation: keep MVP intentionally simple and revisit only after real usage.
- Empty values can be ambiguous. Mitigation: treat null or whitespace-only values as missing.

## Migration Plan

- No database migration is required.
- Add application service and tests.
- Wire service registration through Application DI.

## Open Questions

- None for MVP.

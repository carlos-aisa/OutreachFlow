## Context

Sender profiles already centralize sender identity data, but signature content is not modeled as a first-class, reusable artifact for outgoing emails. Users need rich formatting support and consistent message composition so that each generated draft includes the selected sender signature at the bottom of the email body.

## Goals / Non-Goals

**Goals:**
- Add signature storage as part of sender profile management.
- Support signature payloads in HTML or RTF with explicit format metadata.
- Append signature content during draft composition/rendering.
- Expose API and UI behavior for editing and validating signature format/content.

**Non-Goals:**
- Rich text WYSIWYG editor implementation details.
- Automatic conversion between HTML and RTF.
- Per-template signature override logic.

## Decisions

- Signature fields are added to sender profile aggregate as optional values: signatureFormat and signatureContent.
- Signature format is constrained to Html or Rtf through domain validation and API contract validation.
- Draft composition appends signature after rendered template body using a deterministic separator to keep testability.
- Signature append behavior executes only when signature content exists for the selected sender profile.
- Persistence is updated with EF migration for new sender profile columns.
- Integration tests cover persistence, API behavior, and render composition.

## Risks / Trade-offs

- RTF may render differently between clients -> Mitigation: persist original content and keep appending logic format-aware without transforming payload.
- Storing rich content can increase payload size -> Mitigation: enforce maximum length limits and reject oversized signatures.
- Invalid HTML/RTF can degrade message output -> Mitigation: perform lightweight validation and fail fast with explicit validation errors.

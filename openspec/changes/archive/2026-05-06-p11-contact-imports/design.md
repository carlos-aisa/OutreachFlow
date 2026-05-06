## Context

Manual contact creation is useful, but CSV import is essential for realistic usage. The import workflow must be review-first and deterministic.

## Goals / Non-Goals

**Goals:**

- Parse CSV contact files.
- Preview import results before writing data.
- Detect duplicate contacts by normalized email.
- Assign selected tags during import.
- Track import job status and counts.

**Non-Goals:**

- Excel import.
- Google Contacts or Outlook Contacts import.
- Automatic scraping or enrichment.
- Background import processing.

## Decisions

- Split import into preview and commit steps so the user reviews data before creating contacts.
- Use a structured CSV parser rather than ad hoc string splitting.
- Keep business tags inside OutreachFlow and do not modify external contact systems.
- Persist `ImportJob` records for traceability.

## Risks / Trade-offs

- CSV formats vary. Mitigation: document required columns and return row-level validation errors.
- Imports can introduce poor data. Mitigation: preview duplicates and invalid rows before commit.

## Migration Plan

- Add import job table if persisted in this phase.
- Add parser and preview DTOs.
- Add UI after API behavior is covered by tests.

## Open Questions

- The MVP import format can require explicit column names and UTF-8 CSV.

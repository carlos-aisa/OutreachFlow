## MODIFIED Requirements

### Requirement: Draft generation UI
The system SHALL provide a Blazor draft generation wizard and a campaign workspace that can initiate draft generation after all required campaign prerequisites are complete.

#### Scenario: Generate drafts from wizard
- **WHEN** a user completes recipient, template, sender, attachment, and preview steps
- **THEN** the UI creates drafts and shows the generation result

#### Scenario: Generate drafts from a ready campaign
- **WHEN** a user requests draft generation from a campaign with a message, sender profile, and recipients
- **THEN** the UI creates campaign-owned drafts and shows the generation result

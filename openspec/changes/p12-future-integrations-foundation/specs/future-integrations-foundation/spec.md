## ADDED Requirements

### Requirement: Future provider boundaries
The system SHALL document boundaries for future Gmail, Microsoft Graph, Google Contacts, Outlook Contacts, SMTP, background jobs, PostgreSQL, Docker, and observability integrations.

#### Scenario: Integration boundary documented
- **WHEN** a developer reads the architecture documentation
- **THEN** the documentation explains where future provider implementations belong and which layers they may reference

### Requirement: External contact system safety
The system SHALL NOT automatically modify Gmail, Outlook, Google Contacts, or Outlook Contacts data in early versions.

#### Scenario: External contact sync not implemented
- **WHEN** future integration documentation is reviewed
- **THEN** it states that OutreachFlow tags and business metadata remain inside OutreachFlow unless a later explicit sync change is approved

### Requirement: Provider opt-in configuration
The system SHALL keep future provider integrations disabled unless explicitly configured.

#### Scenario: Provider not configured
- **WHEN** no future provider configuration is present
- **THEN** the system continues using the existing MVP provider behavior

### Requirement: Separate implementation changes
The system SHALL require separate OpenSpec changes before implementing major future integrations.

#### Scenario: Gmail API implementation requested
- **WHEN** Gmail API sending is selected for implementation
- **THEN** a dedicated OpenSpec change defines its requirements, design, tasks, tests, and documentation

### Requirement: Post-MVP infrastructure readiness
The system SHALL document future PostgreSQL, Docker Compose, background jobs, and OpenTelemetry options without changing MVP runtime behavior.

#### Scenario: MVP runtime unchanged
- **WHEN** future integration foundation is implemented
- **THEN** SQLite, fake/SMTP sending, and existing local development commands continue to work

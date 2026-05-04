## ADDED Requirements

### Requirement: SMTP provider configuration
The system SHALL support SMTP provider configuration through environment-based configuration sources.

#### Scenario: Configure SMTP sender
- **WHEN** SMTP configuration values are provided
- **THEN** the system registers the SMTP sender as an available email provider

### Requirement: Secret safety
The system SHALL NOT store SMTP secrets in source-controlled files.

#### Scenario: Missing SMTP password in source files
- **WHEN** repository configuration files are inspected
- **THEN** they contain no real SMTP password or token values

### Requirement: SMTP email sending
The system SHALL send approved draft emails through SMTP when SMTP is the configured provider.

#### Scenario: SMTP send success
- **WHEN** SMTP provider returns a successful send result
- **THEN** the system records the email message as sent with provider `SMTP`

#### Scenario: SMTP send failure
- **WHEN** SMTP provider returns or throws a send failure
- **THEN** the system records the email message as failed with a failure reason

### Requirement: SMTP attachments
The system SHALL include active draft attachments when sending through SMTP.

#### Scenario: Send with attachment
- **WHEN** an approved draft has active attachments
- **THEN** SMTP send command includes those attachment files

### Requirement: Provider-independent tests
The system SHALL keep automated tests independent from real SMTP services.

#### Scenario: Run tests without SMTP server
- **WHEN** `dotnet test` runs in CI
- **THEN** tests pass without contacting a real SMTP server

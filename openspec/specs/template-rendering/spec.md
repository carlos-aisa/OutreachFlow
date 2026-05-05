# template-rendering Specification

## Purpose
TBD - created by archiving change p03-template-rendering-engine. Update Purpose after archive.
## Requirements
### Requirement: Supported variable rendering
The system SHALL render supported template variables in email subject and body content.

#### Scenario: Render contact and sender variables
- **WHEN** a template contains supported contact and sender variables with values in context
- **THEN** the rendered subject and body contain the resolved values

### Requirement: Unknown variable detection
The system SHALL detect variables that are not present in the supported variable registry.

#### Scenario: Unknown variable found
- **WHEN** a template contains `{{contact.unknownField}}`
- **THEN** the render result includes `contact.unknownField` in unknown variables and reports errors

### Requirement: Missing variable value detection
The system SHALL detect supported variables that have no value in the render context.

#### Scenario: Missing organization value found
- **WHEN** a template contains `{{organization.name}}` and the contact has no organization
- **THEN** the render result includes `organization.name` in missing variables and reports errors

### Requirement: Unresolved variable safety
The system SHALL report errors when rendered content would still contain unresolved template variables.

#### Scenario: Template has unresolved content
- **WHEN** a render operation leaves any `{{...}}` token unresolved
- **THEN** the render result reports errors

### Requirement: Simple rendering scope
The system SHALL support variable substitution only and SHALL NOT execute loops, conditionals, scripts, or expressions.

#### Scenario: Unsupported expression syntax
- **WHEN** a template contains expression-like syntax inside a token
- **THEN** the renderer treats it as an unknown variable rather than executing it


# Security Policy

## Reporting a Vulnerability

If you discover a security issue, please do not open a public issue.

Send a private report with reproduction details and impact assessment to the project maintainer through GitHub private communication channels.

Please include:

- A clear description of the vulnerability
- Affected components and versions
- Reproduction steps or proof of concept
- Potential impact
- Suggested mitigation (if available)

## Response Expectations

- Initial acknowledgment target: within 5 business days
- Validation and triage: as soon as reasonably possible
- Fix timeline: based on severity and exploitability

## Security Practices in This Repository

- Do not commit secrets or credentials
- Use environment variables or `dotnet user-secrets` for local sensitive settings
- Keep provider credentials out of source control
- Prefer least-privilege credentials for SMTP or external providers

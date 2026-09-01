## Why

OutreachFlow stores runtime state across the SQLite database and the attachment file storage root, but it does not yet provide a supported recovery workflow that users can access from the application. Once the web workspace gains a dedicated settings page, backup and restore should live there as part of data administration instead of remaining an implicit operational concern.

## What Changes

- Add backup and restore actions to the web settings page for the complete runtime data set.
- Back up the SQLite database and attachment storage together as one validated package.
- Validate uploaded restore packages before replacing live runtime data and require explicit confirmation for recovery actions.
- Document the runtime data layout and operational constraints for installed and development environments.

## Capabilities

### New Capabilities
- `runtime-data-backup-restore`: Backup and restore of the deployed OutreachFlow runtime data set, including the SQLite database and attachment files, exposed through the web settings experience.

### Modified Capabilities
- `attachment-assets`: Clarify that attachment files participate in backup and restore operations as part of the runtime data set.
- `release-installer-packaging`: Clarify the installed runtime data layout that backup and restore operations must target.

## Impact

- Affected code in settings page components, application or infrastructure backup services, and whichever API or server-side workflow coordinates recovery.
- Affects the installed runtime directories under `C:\ProgramData\OutreachFlow` and the development runtime paths configured through `appsettings`.
- Requires behavior tests covering backup packaging, restore validation failures, and safe handling of live runtime data replacement.
- Requires operational documentation for backup creation, restore execution, and recovery constraints.

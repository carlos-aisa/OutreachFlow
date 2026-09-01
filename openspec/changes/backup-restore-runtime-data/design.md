## Context

OutreachFlow persists operational state in two different places: the SQLite database file configured through `ConnectionStrings:OutreachFlow` and the attachment storage root configured through `AttachmentStorage:RootPath`. In installed environments the Windows installer rewrites those paths into `C:\ProgramData\OutreachFlow\data` and `C:\ProgramData\OutreachFlow\attachments`, while development environments keep relative paths under the application content root.

The product direction has now shifted from a purely operational backup idea to a settings-based experience inside the web workspace. That means this change needs to provide a user-facing settings surface while still preserving the safety constraints that originally made backup and restore an operational task. The main complexity is not downloading a package; it is safely restoring a live SQLite-backed runtime plus attachments without corrupting application state.

## Goals / Non-Goals

**Goals:**
- Add backup and restore actions to the web settings page.
- Treat the database and attachment files as one recoverable runtime data set.
- Validate restore packages before modifying live runtime data.
- Keep the design compatible with installed and development runtime layouts.

**Non-Goals:**
- Introducing cloud backup providers, scheduled backups, or off-site synchronization.
- Building a general-purpose file management center beyond backup and restore.
- Changing the database engine or attachment storage model.
- Solving multi-machine coordination beyond a single explicit recovery workflow.

## Decisions

### Use the settings page as the user-facing surface, not as the execution boundary

- **Decision:** Expose backup and restore from the settings page, but keep the actual recovery work in a server-side workflow that owns validation and file-system access.
- **Why:** The settings page is the right discovery point for configuration and recovery actions, but the browser cannot safely coordinate runtime file operations on its own.
- **Alternatives considered:**
  - **Purely operational external tooling:** safer to start with, but weaker UX now that the product is gaining a settings surface.
  - **Client-driven file operations:** not viable because runtime data lives on the server machine and must be validated server-side.

### Package the runtime data set with validation metadata

- **Decision:** The backup artifact should contain the SQLite database, the attachment directory tree, and a metadata manifest describing backup format version, source paths, and creation timestamp.
- **Why:** The manifest makes restore validation explicit and gives future versions a place to record compatibility rules without guessing from file names alone.
- **Alternatives considered:**
  - **Raw directory copy with no metadata:** simpler, but weaker for validation and version checks.
  - **Database-only export:** incomplete because attachment assets live outside SQLite.

### Stage restore input before applying recovery

- **Decision:** Restore should first upload and validate a backup package, then require an explicit confirmation step before applying the recovery workflow to live runtime data.
- **Why:** Recovery is destructive to current live data, so the user needs clear validation feedback and a deliberate confirmation boundary.
- **Alternatives considered:**
  - **Immediate restore on upload:** simpler, but too risky and too opaque for destructive recovery.
  - **Restore the database and attachments independently:** easier to wire, but more likely to create mismatched state.

### Use an exclusive server-side recovery operation for live data replacement

- **Decision:** The implementation should use a single server-side recovery path that prevents overlapping recovery actions and coordinates database plus attachment replacement as one unit.
- **Why:** The biggest failure mode is partial recovery that leaves records and files out of sync or tries to replace runtime files while they are still actively in use.
- **Alternatives considered:**
  - **Best-effort recovery:** unsafe because partial success is difficult to reason about.
  - **Manual restore of separate parts:** pushes too much risk onto the user and undermines the purpose of the settings workflow.

### Reuse the existing configured runtime paths

- **Decision:** Backup and restore should discover the active database path and attachment root from the same configuration values the application already uses.
- **Why:** This avoids introducing another source of truth and keeps installed and development environments on the same conceptual workflow.
- **Alternatives considered:**
  - **Hardcode ProgramData paths:** works for installer deployments only and would be incorrect for development or customized environments.
  - **Separate backup configuration section:** adds flexibility, but increases configuration drift before the basic workflow exists.

## Risks / Trade-offs

- **[Restore of live SQLite data is more complex inside a running web app]** -> Keep recovery server-controlled, enforce an exclusive operation, and validate the full package before applying changes.
- **[Users may underestimate the destructiveness of restore]** -> Add a clear confirmation step and explicit messaging that live data will be replaced.
- **[Large attachment sets produce slow or large backups]** -> Keep the first version focused on correctness and portability rather than incremental or optimized backup formats.
- **[The backup page depends on the settings page foundation from the preferences change]** -> Sequence implementation so the settings shell lands before the backup section is integrated.

## Migration Plan

1. Land the settings-page foundation change that introduces the configuration surface.
2. Add the backup package and restore validation services behind server-side workflows.
3. Add the settings data section for backup download and restore upload plus confirmation.
4. Document the workflow and verify installed and development runtime layouts still point to the intended recovery targets.

## Open Questions

- Should the first restore implementation apply recovery entirely in-process, or should it hand off the final replacement step to a maintenance-oriented server workflow that can briefly restart or quiesce the application?

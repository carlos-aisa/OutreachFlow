## 1. Recovery workflow foundation

- [ ] 1.1 Add shared runtime-data path resolution and backup package metadata support for the configured SQLite database and attachment storage roots.
- [ ] 1.2 Implement the server-side backup workflow that captures the database and attachments as one portable package.

## 2. Settings-based restore experience

- [ ] 2.1 Add a settings-page data section with backup download, restore upload, validation feedback, and explicit restore confirmation.
- [ ] 2.2 Implement the exclusive server-side restore workflow so validated packages replace live runtime data safely as one recovery operation.

## 3. Verification and documentation

- [ ] 3.1 Add automated tests for backup packaging, restore validation failures, and safe handling of invalid paths or partial recovery conditions.
- [ ] 3.2 Update operational and user-facing documentation for runtime data locations, backup usage, and restore constraints.

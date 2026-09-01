## Context

Contact selection currently relies on one-off filters. Campaigns need reusable audiences that remain understandable to a non-technical user while allowing exceptions.

## Goals / Non-Goals

**Goals:**
- Store named groups and calculate their membership from the supported criteria.
- Support several group memberships for each contact and manual include/exclude overrides.
- Define deterministic, simple criterion semantics.

**Non-Goals:**
- Provide arbitrary boolean expressions, nested groups, relative-date filters, or automatic sending.

## Decisions

- Model a group separately from manual overrides. Criteria remain stored on the group; manual inclusions and exclusions use separate contact-group rows so exceptions do not mutate contact data.
- Evaluate selected values within one criterion as OR and different active criteria as AND. Empty criteria do not restrict membership.
- Use only province, city, organization type, and tags in the initial release. These are structured fields; role and source are free text and last-contacted needs more complex date semantics.
- Preserve group membership for do-not-contact and archived contacts, but make those contacts ineligible when a campaign uses a group.

## Risks / Trade-offs

- [Calculated groups can appear surprising] → Display why a contact is included and visually distinguish manual overrides.
- [Organization changes affect a group's result] → Calculate membership at read time and let campaigns snapshot explicit recipients later.
- [Query cost grows] → Index group overrides and use relational queries; optimize only after measuring realistic datasets.

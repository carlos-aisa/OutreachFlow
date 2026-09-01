## 1. Domain and persistence

- [ ] 1.1 Add campaign recipient and message-version domain models with lifecycle state transitions.
- [ ] 1.2 Configure EF Core persistence, uniqueness constraints, indexes, and migration for campaign recipient history.
- [ ] 1.3 Add relational integration tests for candidate discovery, explicit inclusion, duplicate prevention, and safety revalidation.

## 2. Application and API

- [ ] 2.1 Add operations to discover eligible late recipients and explicitly incorporate them into open campaigns.
- [ ] 2.2 Add campaign-recipient draft generation and outcome reporting.
- [ ] 2.3 Integrate recipient lifecycle updates with existing draft approval, send outcome, and follow-up creation flows.
- [ ] 2.4 Add recipient lifecycle endpoints and update OpenAPI contracts.
- [ ] 2.5 Add application and API tests for lifecycle transitions, failures, and do-not-contact exclusions.

## 3. Web experience

- [ ] 3.1 Add campaign recipient queues, candidate discovery, and explicit incorporation actions to the campaign workspace.
- [ ] 3.2 Show recipient state, duplicate protection feedback, and follow-up context without automatic sending.
- [ ] 3.3 Add localization and web component tests for recipient lifecycle interactions.

## 4. Verification and documentation

- [ ] 4.1 Document adding late recipients and the safeguards that prevent duplicate or automatic outreach.
- [ ] 4.2 Run affected domain, application, API, relational integration, web component, and solution build checks.

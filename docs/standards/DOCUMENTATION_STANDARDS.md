description: Standards and best practices for technical documentation in this project, including documentation structure, update processes, and language rules.
globs:

- "**/*.md"
alwaysApply: true

# Rules and Patterns for Documentation

## Documentation

When writing technical documentation such as:

- Data models
- Unit tests documentation
- README files
- API specifications
- Any other Markdown documentation

You MUST ALWAYS write in **English**, including:

- Documentation files
- Code comments
- Function, method, and field explanations

This rule applies both when:

- Creating new documentation
- Updating existing documentation
- Writing documentation within the codebase (comments, explanations, annotations)

Before making any commit or git push, or when explicitly asked to document a change, you MUST always review which technical documentation needs to be updated.

---

## Documentation Updates

When updating documentation, you MUST follow this process:

1. Review all recent changes in the codebase.
2. Identify which documentation files must be updated based on those changes.

   Clear examples include:
   - For **data model changes**:  
     Update the data model definition section in `data-model.md`.
   - For **API changes**:  
     Update `api-spec.yml`.
   - For **library, database, migration, or installation process changes**:  
     Update `*-standards.md` or the relevant installation or configuration documentation.

3. Update each affected documentation file **in English**, maintaining consistency with existing documentation.
4. Ensure all documentation is properly formatted and follows the established structure.
5. Verify that all changes are accurately reflected in the documentation.
6. Report which files were updated and summarize what changes were made.

---

## Documentation Quality Rules

- Documentation must be:
  - Clear
  - Explicit
  - Consistent
  - Up to date
- Avoid ambiguous language.
- Avoid assumptions about reader knowledge.
- Prefer explicit explanations over implicit behavior.
- Keep documentation aligned with the actual implementation.

---

## AI-Specific Rules

- The AI must not assume documentation is up to date.
- The AI must proactively identify missing or outdated documentation.
- The AI must not skip documentation updates when code changes affect behavior, APIs, configuration, or data models.
- If documentation requirements are unclear, the AI must ask for clarification before proceeding.

Documentation is considered part of the deliverable, not an optional task.

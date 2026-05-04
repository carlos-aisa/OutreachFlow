## 1. Contracts

- [ ] 1.1 Add `ITemplateRenderer` application interface.
- [ ] 1.2 Add `TemplateContext` and `RenderedEmail` application models.
- [ ] 1.3 Add supported variable registry and tests for variable names.

## 2. Rendering Implementation

- [ ] 2.1 Implement token parsing for `{{variable.path}}`.
- [ ] 2.2 Implement subject and body rendering.
- [ ] 2.3 Implement unknown variable detection.
- [ ] 2.4 Implement missing variable value detection.
- [ ] 2.5 Register the renderer in Application DI.

## 3. Tests

- [ ] 3.1 Add tests for successful rendering.
- [ ] 3.2 Add tests for unknown variables.
- [ ] 3.3 Add tests for missing values.
- [ ] 3.4 Add tests for unresolved tokens and unsupported expression syntax.

## 4. Verification

- [ ] 4.1 Run `dotnet test`.
- [ ] 4.2 Update documentation for supported template variables.

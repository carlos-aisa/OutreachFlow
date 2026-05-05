## 1. Contracts

- [x] 1.1 Add `ITemplateRenderer` application interface.
- [x] 1.2 Add `TemplateContext` and `RenderedEmail` application models.
- [x] 1.3 Add supported variable registry and tests for variable names.

## 2. Rendering Implementation

- [x] 2.1 Implement token parsing for `{{variable.path}}`.
- [x] 2.2 Implement subject and body rendering.
- [x] 2.3 Implement unknown variable detection.
- [x] 2.4 Implement missing variable value detection.
- [x] 2.5 Register the renderer in Application DI.

## 3. Tests

- [x] 3.1 Add tests for successful rendering.
- [x] 3.2 Add tests for unknown variables.
- [x] 3.3 Add tests for missing values.
- [x] 3.4 Add tests for unresolved tokens and unsupported expression syntax.

## 4. Verification

- [x] 4.1 Run `dotnet test`.
- [x] 4.2 Update documentation for supported template variables.

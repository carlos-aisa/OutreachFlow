## 1. Installer Toolchain

- [x] 1.1 Add WiX MSI project for OutreachFlow payload and install wizard.
- [x] 1.2 Add WiX bootstrapper project for setup executable.
- [x] 1.3 Add installer build script that publishes API and Web payload and builds MSI/setup artifacts.

## 2. Service Configuration

- [x] 2.1 Add installer custom action scripts to configure runtime data paths.
- [x] 2.2 Add installer custom action scripts to register/start API and Web Windows services.
- [x] 2.3 Add uninstall script to stop/delete services and optionally remove runtime data.

## 3. Application Hosting Adjustments

- [x] 3.1 Enable Windows service hosting mode in API and Web startup.
- [x] 3.2 Make HTTPS redirection behavior configurable for service-hosted deployments.

## 4. Release Pipeline and Docs

- [x] 4.1 Update release workflow to build and validate setup.exe + MSI assets.
- [x] 4.2 Update installer release documentation and README release section.
- [x] 4.3 Update changelog entry for installer strategy transition.

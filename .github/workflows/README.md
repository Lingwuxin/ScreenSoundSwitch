# GitHub Workflows

This directory contains GitHub Actions workflows for automated building and packaging of the ScreenSoundSwitch application.

## Workflows

### 1. Build and Package (`build-and-package.yml`)

This workflow automatically builds and packages the ScreenSoundSwitch WinUI application for distribution.

**Triggers:**
- When a tag starting with `v` is pushed (e.g., `v1.0.0`)
- When a release is published
- Manual trigger via GitHub UI with configurable build configuration

**Features:**
- Builds for multiple architectures: x86, x64, ARM64
- Creates MSIX packages for easy installation
- Uploads build artifacts for each platform
- Automatically creates GitHub releases for tagged versions
- Supports Release, Debug, and Nightly build configurations

**Usage:**
1. Create a tag: `git tag v1.0.0 && git push origin v1.0.0`
2. Or manually trigger from GitHub Actions tab

### 2. Continuous Integration (`ci.yml`)

This workflow runs on every push and pull request to ensure code quality.

**Triggers:**
- Push to main/master/develop branches
- Pull requests targeting main/master/develop branches

**Features:**
- Quick build verification for x64 platform
- Runs available tests
- Provides fast feedback for development

## Build Artifacts

The workflows generate the following artifacts:

- **MSIX Packages**: Ready-to-install packages for Windows
- **Executable Files**: Standalone executables for each platform
- **Retention**: Artifacts are kept for 30 days

## Requirements

- Windows runner (for building Windows-specific applications)
- .NET 8.0 SDK
- MSBuild tools
- MSIX tooling enabled

## Notes

- The application requires Windows 10 version 17763 or later
- ARM64 builds are included for modern Windows devices
- All packages are created as sideload-only (no Microsoft Store submission)
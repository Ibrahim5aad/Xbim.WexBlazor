# Version Management

This project uses **git tags** for automatic version management. You don't need to manually update version numbers in the code.

## Publishing to NuGet.org

To publish a new version to NuGet.org:

1. **Create and push a git tag:**
   ```bash
   git tag v1.0.6
   git push origin v1.0.6
   ```

2. **GitHub Actions automatically:**
   - Extracts version from the tag (removes the `v` prefix)
   - Updates the `.csproj` with that version
   - Compiles TypeScript
   - Builds the project
   - Creates the NuGet package
   - Publishes to NuGet.org

## Publishing to GitHub Packages (develop branch)

When you push to the `develop` branch:

1. **Push your changes:**
   ```bash
   git checkout develop
   git push
   ```

2. **GitHub Actions automatically:**
   - Generates a dev version: `0.0.0-dev.YYYYMMDD.commithash`
   - Updates the `.csproj` with that version
   - Publishes to GitHub Packages

## Version Format

### Production Releases (NuGet.org)
Use semantic versioning: `v{major}.{minor}.{patch}`

Examples:
- `v1.0.6` - Patch release (bug fixes)
- `v1.1.0` - Minor release (new features, backward compatible)
- `v2.0.0` - Major release (breaking changes)

### Pre-release Versions
Add a suffix for pre-releases:
- `v1.0.6-alpha`
- `v1.0.6-beta.1`
- `v1.0.6-rc.1`

### Development Builds (GitHub Packages)
Automatically generated: `0.0.0-dev.{date}.{commit}`

## Workflow Summary

**For NuGet.org (production):**
```
git tag v1.0.6 → GitHub Actions → NuGet.org
```

**For GitHub Packages (development):**
```
git push develop → GitHub Actions → GitHub Packages
```

## Current Version

The version in `src/Xbim.WexBlazor/Xbim.WexBlazor.csproj` is a placeholder. The actual version is set by GitHub Actions during the build based on the git tag.

## Manual Trigger

You can also manually trigger the workflow from the GitHub Actions tab if needed.

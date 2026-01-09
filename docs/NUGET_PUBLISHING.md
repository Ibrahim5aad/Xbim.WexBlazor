# Publishing Xbim.WexBlazor to NuGet

This guide explains how to publish the `Xbim.WexBlazor` package to NuGet.org and GitHub Packages using GitHub Actions.

## Prerequisites

### 1. Get a NuGet.org API Key

1. Go to [NuGet.org](https://www.nuget.org/) and sign in (or create an account)
2. Click your username (top-right) → **API Keys**
3. Click **Create** to generate a new API key
4. Configure the key:
   - **Key Name**: `GitHub Actions - Xbim.WexBlazor`
   - **Glob Pattern**: `Xbim.WexBlazor`
   - **Expiration**: Choose your preferred duration (or no expiration)
   - **Scopes**: Select **Push** and **Push new packages and package versions**
5. Click **Create**
6. **IMPORTANT**: Copy the API key immediately - you won't be able to see it again!

### 2. Add API Key to GitHub Secrets

1. Go to your GitHub repository: `https://github.com/Ibrahim5aad/Xbim.WexBlazor`
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: Paste your NuGet.org API key
6. Click **Add secret**

## Publishing Methods

### Method 1: Publish via GitHub Release (Recommended)

This is the easiest and most controlled way to publish:

1. **Update the version** in `src/Xbim.WexBlazor/Xbim.WexBlazor.csproj`:
   ```xml
   <Version>1.0.0</Version>
   ```

2. **Commit and push** your changes:
   ```bash
   git add .
   git commit -m "chore: bump version to 1.0.0"
   git push
   ```

3. **Create a GitHub Release**:
   - Go to your repository → **Releases** → **Draft a new release**
   - Click **Choose a tag** → Type `v1.0.0` → **Create new tag: v1.0.0 on publish**
   - **Release title**: `v1.0.0`
   - **Description**: Add release notes describing changes
   - Click **Publish release**

4. The GitHub Action will automatically:
   - Build the project
   - Create the NuGet package
   - Publish to NuGet.org

5. **Monitor the workflow**:
   - Go to **Actions** tab to see the progress
   - The package should appear on NuGet.org within a few minutes

### Method 2: Manual Workflow Dispatch

If you need to publish without creating a release:

1. Go to **Actions** tab in GitHub
2. Select **Publish to NuGet** workflow
3. Click **Run workflow**
4. Enter the version number (e.g., `1.0.0`)
5. Click **Run workflow**

### Method 3: Local Publishing (Manual)

For testing or manual control:

```bash
# Navigate to project directory
cd src/Xbim.WexBlazor

# Build the project
dotnet build --configuration Release

# Create the NuGet package
dotnet pack --configuration Release --output ./nupkg

# Push to NuGet.org
dotnet nuget push ./nupkg/*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## Publishing to GitHub Packages (Alternative)

GitHub Packages is automatically set up and requires no API key configuration:

1. **Create and push a version tag**:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The workflow will automatically publish to GitHub Packages

3. **Users can consume from GitHub Packages** by adding to their `nuget.config`:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <packageSources>
       <add key="github" value="https://nuget.pkg.github.com/Ibrahim5aad/index.json" />
     </packageSources>
     <packageSourceCredentials>
       <github>
         <add key="Username" value="YOUR_GITHUB_USERNAME" />
         <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
       </github>
     </packageSourceCredentials>
   </configuration>
   ```

## Version Management Best Practices

### Semantic Versioning

Follow [Semantic Versioning](https://semver.org/) (MAJOR.MINOR.PATCH):

- **MAJOR** (1.0.0 → 2.0.0): Breaking changes
- **MINOR** (1.0.0 → 1.1.0): New features, backward compatible
- **PATCH** (1.0.0 → 1.0.1): Bug fixes, backward compatible

### Pre-release Versions

For alpha/beta releases, use suffixes:

```xml
<Version>1.0.0-alpha</Version>
<Version>1.0.0-beta.1</Version>
<Version>1.0.0-rc.1</Version>
```

Pre-release packages won't be installed by default unless explicitly requested.

## Verifying Publication

After publishing:

1. **Check NuGet.org**: Visit `https://www.nuget.org/packages/Xbim.WexBlazor/`
2. **Search**: Wait 5-10 minutes, then search for "Xbim.WexBlazor" on NuGet.org
3. **Install**: Test installation in a new project:
   ```bash
   dotnet add package Xbim.WexBlazor
   ```

## Troubleshooting

### Package Already Exists
If you see "Package already exists" error:
- You cannot overwrite an existing version on NuGet.org
- Increment the version number in `.csproj`
- Delete the version from NuGet.org if needed (within 72 hours of publishing)

### API Key Invalid
- Verify the secret name is exactly `NUGET_API_KEY`
- Check the API key hasn't expired on NuGet.org
- Regenerate the API key if needed

### Build Failures
- Ensure all TypeScript compiles correctly
- Check that libman restored the xbim-viewer library
- Verify .NET 9.0 SDK is installed in the workflow

## License Considerations

The package is published under the MIT License. Ensure:
- Your `LICENSE` file is present in the repository
- The `PackageLicenseExpression` in `.csproj` is correct
- You have the right to publish any dependencies

## Support

For issues with publishing, check:
- [NuGet.org Publishing Docs](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- Repository Issues: `https://github.com/Ibrahim5aad/Xbim.WexBlazor/issues`

# Quick Start: Publishing to NuGet

## 🚀 TL;DR - Get Your Package on NuGet in 5 Minutes

### 1. Get NuGet API Key
1. Sign in to [NuGet.org](https://www.nuget.org/)
2. Go to **Your Username** → **API Keys** → **Create**
3. Set Glob Pattern: `Xbim.WexBlazor`
4. Select **Push** permissions
5. Copy the API key (you can't see it again!)

### 2. Add API Key to GitHub
1. Go to `https://github.com/Ibrahim5aad/Xbim.WexBlazor/settings/secrets/actions`
2. Click **New repository secret**
3. Name: `NUGET_API_KEY`
4. Paste your API key → **Add secret**

### 3. Publish Your Package

#### Option A: Create a GitHub Release (Easiest ✅)
```bash
# 1. Update version in src/Xbim.WexBlazor/Xbim.WexBlazor.csproj
# Change: <Version>1.0.0</Version> to <Version>1.0.1</Version>

# 2. Commit and push
git add .
git commit -m "chore: bump version to 1.0.1"
git push

# 3. Create Release on GitHub
# Go to: https://github.com/Ibrahim5aad/Xbim.WexBlazor/releases/new
# - Tag: v1.0.1
# - Title: v1.0.1
# - Description: Add your release notes
# - Click "Publish release"
```

That's it! The package will be on NuGet.org in ~5 minutes.

#### Option B: Manual Workflow Trigger
1. Go to **Actions** tab → **Publish to NuGet**
2. Click **Run workflow**
3. Enter version → **Run workflow**

## 📦 What Was Set Up

### Files Created
- `.github/workflows/publish-nuget.yml` - Auto-publish to NuGet.org
- `.github/workflows/publish-github-packages.yml` - Auto-publish to GitHub Packages
- `docs/NUGET_PUBLISHING.md` - Detailed documentation
- `LICENSE` - MIT License file

### Project Updated
- `src/Xbim.WexBlazor/Xbim.WexBlazor.csproj` - Added NuGet metadata:
  - Package ID, version, description
  - Authors, license, repository info
  - README inclusion
  - Symbol package generation

## 🔍 Verify Your Package

After publishing, check:
```bash
# Search on NuGet.org (wait 5-10 minutes)
https://www.nuget.org/packages/Xbim.WexBlazor/

# Test installation
dotnet new blazorwasm -n TestApp
cd TestApp
dotnet add package Xbim.WexBlazor
```

## 📝 Version Bumping

**Before each release, update the version:**

```xml
<!-- src/Xbim.WexBlazor/Xbim.WexBlazor.csproj -->
<Version>1.0.0</Version>  <!-- Change this -->
```

**Versioning Guidelines:**
- `1.0.0` → `1.0.1`: Bug fixes
- `1.0.0` → `1.1.0`: New features
- `1.0.0` → `2.0.0`: Breaking changes
- `1.0.0-alpha`: Pre-release

## ⚠️ Important Notes

1. **Cannot overwrite versions**: Once published, you can't replace `1.0.0` with a different `1.0.0`
2. **Deletion window**: You have 72 hours to unlist/delete a version on NuGet.org
3. **API Key security**: Never commit API keys to your repository
4. **Build status**: Check the **Actions** tab for workflow status

## 🆘 Common Issues

**"Package already exists"**
→ Bump the version number in `.csproj`

**"API key invalid"**
→ Check GitHub secret name is `NUGET_API_KEY`
→ Verify key hasn't expired on NuGet.org

**Build fails in workflow**
→ Check TypeScript compiles locally
→ Ensure libman restored xbim-viewer

## 📚 Full Documentation

See [`docs/NUGET_PUBLISHING.md`](./NUGET_PUBLISHING.md) for complete details.

## 🎉 Success!

Once published, users can install your package:
```bash
dotnet add package Xbim.WexBlazor
```

```html
@using Xbim.WexBlazor.Components

<XbimViewerComponent />
```

---

**Need Help?** Open an issue: https://github.com/Ibrahim5aad/Xbim.WexBlazor/issues

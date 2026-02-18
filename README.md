# Xbim.WexBlazor

[![GitHub Packages](https://img.shields.io/badge/GitHub%20Packages-Xbim.WexBlazor-blue)](https://github.com/Ibrahim5aad/Xbim/pkgs/nuget/Xbim.WexBlazor)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Blazor component library for building BIM (Building Information Modeling) applications. Wraps the [@xbim/viewer](https://www.npmjs.com/package/@xbim/viewer) JavaScript library for 3D model visualization in Blazor WebAssembly or Server applications.

## Features

- **3D BIM Viewer** - WebGL-based visualization of wexBIM models
- **Plugin System** - Navigation cube, grid, section box, clipping planes
- **Sidebar Docking** - Dockable/overlay panels with icon bar navigation
- **Property Display** - Multi-source property aggregation (IFC, database, custom)
- **Model Hierarchy** - Product types and spatial structure navigation
- **Theming** - Light/dark themes with customizable colors
- **Direct IFC Loading** - Server-side conversion (Blazor Server only)

## Two Modes of Operation

This library supports two modes to fit different application needs:

| Mode | Description | Best For |
|------|-------------|----------|
| **Standalone** | Self-contained viewer with no backend dependencies | Demos, embedded viewers, simple apps, prototypes |
| **Platform** | Integrated with Xbim Server for full model management | Production apps, team collaboration, cloud storage |

## Installation

```bash
# Add GitHub Packages source (one-time setup)
dotnet nuget add source https://nuget.pkg.github.com/Ibrahim5aad/index.json --name github --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_PAT

# Install the package
dotnet add package Xbim.WexBlazor
```

> **Note:** The `YOUR_GITHUB_PAT` needs `read:packages` scope. [Create a PAT here](https://github.com/settings/tokens).

For platform mode, also install the API client:

```bash
dotnet add package Xbim.WexServer.Client
```

## Quick Start

### Standalone Mode

Register services for standalone operation (no server required):

```csharp
// Program.cs
builder.Services.AddXbimBlazorStandalone();
```

Add to `_Imports.razor`:

```razor
@using Xbim.WexBlazor
@using Xbim.WexBlazor.Components
```

Basic viewer:

```razor
<XbimViewer Id="viewer"
               Width="800"
               Height="600"
               ModelUrl="models/building.wexbim"
               OnModelLoaded="HandleModelLoaded" />

@code {
    private void HandleModelLoaded(bool success)
    {
        Console.WriteLine(success ? "Model loaded" : "Load failed");
    }
}
```

### Platform Mode

Register services with Xbim Server connection:

```csharp
// Program.cs
builder.Services.AddXbimClient(options =>
{
    options.BaseUrl = "https://your-Xbim-server.com";
});
builder.Services.AddXbimBlazorPlatform();
```

Platform mode enables:
- Model storage in cloud or on-premises
- Model versioning and history
- Property extraction stored in database
- User authentication and workspace management

## Components

### XbimViewer

The main viewer component with full model interaction support.

```razor
<XbimViewer Id="myViewer"
               Width="100%"
               Height="600"
               BackgroundColor="#F5F5F5"
               ModelUrl="models/SampleModel.wexbim"
               OnViewerInitialized="HandleInit"
               OnModelLoaded="HandleLoad"
               OnPick="HandlePick">
    <!-- Child components go here -->
</XbimViewer>
```

**Parameters:**
- `Id` - Unique viewer identifier
- `Width`/`Height` - Dimensions (px or %)
- `BackgroundColor` - Canvas background
- `ModelUrl` - Initial model URL to load

**Events:**
- `OnViewerInitialized` - Viewer ready
- `OnModelLoaded` - Model load complete
- `OnPick` - Element selected
- `OnHoverPick` - Element hovered

### ViewerToolbar

Built-in toolbar with common viewer operations.

```razor
<XbimViewer ...>
    <ViewerToolbar Position="ToolbarPosition.Top"
                   ShowResetView="true"
                   ShowZoomControls="true"
                   ShowNavigationModes="true" />
</XbimViewer>
```

### ViewerSidebar + SidebarPanel

Dockable sidebar system with icon-based panel management.

```razor
<XbimViewer ...>
    <ViewerSidebar Position="SidebarPosition.Right" DefaultMode="SidebarMode.Docked">
        <SidebarPanel Title="Properties" Icon="bi-info-circle">
            <PropertiesPanel ShowHeader="false" />
        </SidebarPanel>
        <SidebarPanel Title="Hierarchy" Icon="bi-diagram-3">
            <ModelHierarchyPanel ShowHeader="false" />
        </SidebarPanel>
    </ViewerSidebar>
</XbimViewer>
```

### PropertiesPanel

Displays element properties when selected. Auto-subscribes to viewer pick events.

```razor
<PropertiesPanel ShowHeader="true" />
```

### ModelHierarchyPanel

Shows model structure with Product Types and Spatial Structure tabs.

```razor
<ModelHierarchyPanel ShowHeader="true" />
```

### FileLoaderPanel

UI for loading models from URLs, files, or demo assets.

```razor
<FileLoaderPanel AllowIfcFiles="true" OnFileLoaded="HandleFile" />
```

## Plugins

Add viewer plugins for enhanced functionality:

```razor
<XbimViewer @ref="_viewer" ...>
    <NavigationCubePlugin Opacity="0.7" />
    <GridPlugin Spacing="1000" Color="#CCCCCC" />
    <SectionBoxPlugin />
    <ClippingPlanePlugin />
</XbimViewer>
```

## Theming

Register and configure the theme service:

```csharp
// Program.cs
var themeService = new ThemeService();
themeService.SetTheme(ViewerTheme.Dark);
themeService.SetAccentColors(lightColor: "#0969da", darkColor: "#4da3ff");
builder.Services.AddSingleton(themeService);
```

Toggle theme at runtime:

```razor
@inject ThemeService ThemeService

<button @onclick="() => ThemeService.ToggleTheme()">Toggle Theme</button>
```

## Property Sources

The library supports multiple property sources for element data.

### IFC Property Source (Blazor Server)

```csharp
var model = IfcStore.Open("model.ifc");
var propertySource = new IfcPropertySource(model, viewerModelId);
propertyService.RegisterSource(propertySource);
```

### Custom Property Source

```csharp
var apiSource = new CustomPropertySource(
    async (query, ct) =>
    {
        var data = await api.GetPropertiesAsync(query.ElementId);
        return new ElementProperties { /* ... */ };
    },
    sourceType: "REST API",
    name: "API Properties"
);
propertyService.RegisterSource(apiSource);
```

### Dictionary Property Source

```csharp
var dictSource = new DictionaryPropertySource(name: "Custom");
dictSource.AddProperty(elementId: 123, modelId: 0,
    groupName: "Status", propertyName: "Approved", value: "Yes");
propertyService.RegisterSource(dictSource);
```

## IFC Loading (Blazor Server Only)

Direct IFC file loading with automatic wexBIM conversion:

```csharp
// Program.cs
builder.Services.AddSingleton<IfcModelService>();
builder.Services.AddSingleton<IfcHierarchyService>();
```

```csharp
@inject IfcModelService IfcService

var result = await IfcService.ProcessIfcBytesAsync(ifcData, "model.ifc");
if (result.Success)
{
    await Viewer.LoadModelFromBytesAsync(result.WexbimData!, "model.ifc");
}
```

## Service Registration

### Standalone Mode

For viewer applications without a backend server:

```csharp
builder.Services.AddXbimBlazorStandalone();
```

With configuration:

```csharp
builder.Services.AddXbimBlazorStandalone(options =>
{
    options.DefaultTheme = ViewerTheme.Dark;
});
```

### Platform Mode

For applications connected to Xbim Server:

```csharp
builder.Services.AddXbimClient(options =>
{
    options.BaseUrl = "https://your-server.com";
});
builder.Services.AddXbimBlazorPlatform();
```

Platform mode automatically configures property sources to fetch from the server and enables cloud-based model loading.

## Requirements

- .NET 9.0+
- Blazor WebAssembly or Blazor Server

## License

MIT

## Related Packages

- [Xbim.WexServer.Client](https://github.com/Ibrahim5aad/Xbim/pkgs/nuget/Xbim.WexServer.Client) - API client for Xbim Server

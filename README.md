# Xbim.WexBlazor

[![NuGet](https://img.shields.io/nuget/v/Xbim.WexBlazor.svg?style=flat-square)](https://www.nuget.org/packages/Xbim.WexBlazor/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Xbim.WexBlazor.svg?style=flat-square)](https://www.nuget.org/packages/Xbim.WexBlazor/)
[![Build Status](https://github.com/Ibrahim5aad/Xbim.WexBlazor/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/Ibrahim5aad/Xbim.WexBlazor/actions/workflows/publish-nuget.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Blazor component library that wraps the @xbim/viewer JavaScript library for use in Blazor applications. This library allows you to display 3D building models in the wexBIM format. **Blazor Server applications can also load IFC files directly**, which are automatically converted to wexBIM format for visualization.

![Xbim.WexBlazor in action](screenshot.png)

## Project Structure

- **Xbim.WexBlazor**: The component library project
  - **Components/**: Blazor components
  - **Interop/**: JavaScript interop services
  - **wwwroot/js/**: JavaScript interop modules
  - **wwwroot/lib/**: Third-party libraries (xBIM Viewer)

- **Xbim.WexBlazor.Sample**: WebAssembly sample application showcasing the library
- **Xbim.WexBlazor.Server.Sample**: Blazor Server sample application with IFC file loading support

## Features

- Base JavaScript interop infrastructure
- Structured approach to wrapping JavaScript libraries
- xBIM Viewer component for displaying wexBIM 3D models
- Controls for loading models, zooming, and manipulating the view
- **Direct IFC file loading in Blazor Server applications** - automatically converts IFC files to wexBIM format
- Extensible properties system for displaying element properties from multiple sources
- Theme support (light/dark) with customizable accent colors

## Installation

Install the package from NuGet:

```bash
dotnet add package Xbim.WexBlazor
```

Or via the Visual Studio Package Manager:

```
Install-Package Xbim.WexBlazor
```

## Getting Started

### Using the Components

Then add the following to your `_Imports.razor` file:

```razor
@using Xbim.WexBlazor
@using Xbim.WexBlazor.Components
```

Example usage of the xBIM Viewer component:

```razor
<XbimViewerComponent Id="myViewer"
                    Width="800"
                    Height="600"
                    BackgroundColor="#F5F5F5"
                    ModelUrl="models/SampleModel.wexbim"
                    OnViewerInitialized="HandleViewerInitialized"
                    OnModelLoaded="HandleModelLoaded" />

@code {
    private async Task HandleViewerInitialized(string viewerId)
    {
        // The viewer has been initialized
    }
    
    private async Task HandleModelLoaded(bool success)
    {
        // The model has been loaded
    }
}
```

## Theming and Customization

The library supports light and dark themes with customizable accent colors.

### Setting Up the Theme Service

Register the `ThemeService` in your `Program.cs`:

```csharp
using Xbim.WexBlazor.Services;
using Xbim.WexBlazor.Models;

var themeService = new ThemeService();
themeService.SetTheme(ViewerTheme.Dark);
themeService.SetAccentColors(
    lightColor: "#0969da",
    darkColor: "#4da3ff"
);
builder.Services.AddSingleton(themeService);
```

### Using the Theme Service

Inject and use the theme service in your components:

```razor
@inject ThemeService ThemeService

<button @onclick="ToggleTheme">Toggle Theme</button>

@code {
    private void ToggleTheme()
    {
        ThemeService.ToggleTheme();
    }
    
    protected override async Task OnInitializedAsync()
    {
        ThemeService.OnThemeChanged += StateHasChanged;
    }
}
```

### Customizing Accent Colors

You can change the accent colors at runtime:

```csharp
ThemeService.SetAccentColors(
    lightColor: "#d73a49",
    darkColor: "#ff6b6b"
);
```

The accent color is used for:
- Highlighted elements
- Active buttons
- Selected navigation modes
- Button group labels
- Focus indicators

### Customizing Selection and Hover Colors

You can customize the colors used when elements are selected or hovered:

```csharp
ThemeService.SetSelectionAndHoverColors(
    selectionColor: "#ff6b6b",  // Red for selection
    hoverColor: "#4da3ff"        // Blue for hover
);
```

## Element Properties

The library provides a flexible properties system that allows displaying element properties from various sources.

### Property Sources

Property sources provide element properties from different data sources:

1. **IfcPropertySource**: Reads properties directly from IFC models using xBIM Essentials
2. **DictionaryPropertySource**: Stores properties in memory (great for demos or caching)
3. **CustomPropertySource**: Integrates with any custom data source (databases, APIs, etc.)

### Setting Up IFC Property Source

To read properties from IFC files, you need to load the IFC model using xBIM Essentials:

```csharp
using Xbim.Ifc;
using Xbim.WexBlazor.Services;

// Load IFC model
var model = IfcStore.Open("path/to/model.ifc");

// Create property source (viewerModelId is the ID returned when loading the wexbim file)
var propertySource = new IfcPropertySource(model, viewerModelId);

// Register with the property service
propertyService.RegisterSource(propertySource);
```

### Using the Properties Panel

Add the `PropertiesPanel` component to display properties:

```razor
<PropertiesPanel 
    IsVisible="@_showProperties"
    Properties="@_currentProperties"
    IsLoading="@_isLoadingProperties"
    Position="PropertiesPanelPosition.Right" />
```

### Custom Property Source Example

Create a custom property source for integration with databases or APIs:

```csharp
var customSource = new CustomPropertySource(
    propertyProvider: async (query, ct) =>
    {
        // Fetch properties from your database/API
        var data = await myDatabase.GetElementPropertiesAsync(query.ElementId);
        
        return new ElementProperties
        {
            ElementId = query.ElementId,
            ModelId = query.ModelId,
            Name = data.Name,
            Groups = new List<PropertyGroup>
            {
                new PropertyGroup
                {
                    Name = "Custom Properties",
                    Source = "Database",
                    Properties = data.Properties.Select(p => new PropertyValue
                    {
                        Name = p.Key,
                        Value = p.Value
                    }).ToList()
                }
            }
        };
    },
    sourceType: "Database",
    name: "My Database Properties"
);

propertyService.RegisterSource(customSource);
```

### In-Memory Properties (Dictionary Source)

For simple use cases or demos:

```csharp
var dictSource = new DictionaryPropertySource(name: "Demo Properties");

// Add properties for specific elements
dictSource.AddProperty(elementId: 123, modelId: 0, 
    groupName: "Custom", 
    propertyName: "Status", 
    value: "Approved");

propertyService.RegisterSource(dictSource);
```

## Loading IFC Files Directly

**⚠️ Important: IFC file loading is only available in Blazor Server applications**, not in Blazor WebAssembly.

The library supports loading IFC files directly and automatically converting them to wexBIM format for visualization. This functionality uses the [xBIM Geometry Engine v6](https://github.com/xBimTeam/XbimGeometry/tree/feature/netcore) and requires server-side execution (Blazor Server or ASP.NET Core API).

### Why Server-Side Only?

IFC processing requires native code from the xBIM Geometry Engine, which cannot run in WebAssembly browsers. For Blazor WebAssembly applications, you'll need to:
- Pre-convert IFC files to wexBIM format, or
- Create a server-side API endpoint to process IFC files and return wexbim data

### Setting Up IFC Loading (Blazor Server)

```csharp
// In Program.cs
builder.Services.AddSingleton<IfcModelService>();
builder.Services.AddSingleton<PropertyService>();
```

### Processing IFC Files

```csharp
@inject IfcModelService IfcService
@inject PropertyService PropertyService

@code {
    private async Task LoadIfcFile(byte[] ifcData, string fileName)
    {
        // Process the IFC file - generates wexbim and keeps IModel for properties
        var result = await IfcService.ProcessIfcBytesAsync(
            ifcData, 
            fileName,
            new Progress<IfcProcessingProgress>(p => 
            {
                Console.WriteLine($"{p.Stage}: {p.Message} ({p.PercentComplete}%)");
            }));
        
        if (result.Success && result.WexbimData != null)
        {
            // Load the generated wexbim into the viewer
            var loadedModel = await Viewer.LoadModelFromBytesAsync(
                result.WexbimData, 
                fileName);
            
            if (loadedModel != null && result.Model != null)
            {
                // Store the IFC model reference for property access
                loadedModel.IfcModel = result.Model;
                loadedModel.OriginalFormat = ModelFormat.Ifc;
                
                // Register property source for this model
                var propertySource = new IfcPropertySource(result.Model, loadedModel.Id);
                PropertyService.RegisterSource(propertySource);
            }
        }
    }
}
```

### FileLoaderPanel with IFC Support

The `FileLoaderPanel` component automatically supports IFC files when `AllowIfcFiles="true"`:

```razor
<FileLoaderPanel 
    IsVisible="true"
    AllowIfcFiles="true"
    OnFileLoaded="HandleFileLoaded" />
```

The panel will:
- Accept `.ifc`, `.ifczip`, and `.wexbim` files
- Show appropriate icons and badges for each format
- Display a note when IFC files will be processed

### Important Notes

- **✅ Blazor Server**: Full IFC support - can load and process IFC files directly
- **❌ Blazor WebAssembly**: IFC processing not supported - must use pre-converted wexBIM files or a server API
- **Memory Usage**: Large IFC files may require significant memory during processing
- **Processing Time**: Complex models can take several seconds to process
- **Properties**: When loading IFC files, properties are automatically extracted and available in the properties panel

## wexBIM Format

The viewer requires 3D models in the wexBIM format. You can convert IFC models to wexBIM using tools from the [XbimEssentials](https://github.com/xBimTeam/XbimEssentials) library, or use the built-in `IfcModelService` (server-side only).

## Development

To build and run the sample project:

```bash
git clone https://github.com/Ibrahim5aad/Xbim.WexBlazor.git
cd Xbim.WexBlazor
dotnet build
dotnet run --project src/Xbim.WexBlazor.Sample/Xbim.WexBlazor.Sample.csproj
```

## License

MIT

## Acknowledgements

This project uses the [@xbim/viewer](https://www.npmjs.com/package/@xbim/viewer) JavaScript library from the [xBimTeam](https://github.com/xBimTeam/XbimWebUI). 

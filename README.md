# Xbim.WexBlazor

[![NuGet](https://img.shields.io/nuget/v/Xbim.WexBlazor.svg?style=flat-square)](https://www.nuget.org/packages/Xbim.WexBlazor/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Xbim.WexBlazor.svg?style=flat-square)](https://www.nuget.org/packages/Xbim.WexBlazor/)
[![Build Status](https://github.com/Ibrahim5aad/Xbim.WexBlazor/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/Ibrahim5aad/Xbim.WexBlazor/actions/workflows/publish-nuget.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A Blazor component library that wraps the @xbim/viewer JavaScript library for use in Blazor applications. This library allows you to display 3D building models in the wexBIM format.

## Screenshot

![Xbim.WexBlazor in action](screenshot.png)

## Project Structure

- **Xbim.WexBlazor**: The component library project
  - **Components/**: Blazor components
  - **Interop/**: JavaScript interop services
  - **wwwroot/js/**: JavaScript interop modules
  - **wwwroot/lib/**: Third-party libraries (xBIM Viewer)

- **Xbim.WexBlazor.Sample**: Sample application showcasing the library

## Features

- Base JavaScript interop infrastructure
- Structured approach to wrapping JavaScript libraries
- xBIM Viewer component for displaying wexBIM 3D models
- Controls for loading models, zooming, and manipulating the view

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

## wexBIM Format

The viewer requires 3D models in the wexBIM format. You can convert IFC models to wexBIM using tools from the [XbimEssentials](https://github.com/xBimTeam/XbimEssentials) library.

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
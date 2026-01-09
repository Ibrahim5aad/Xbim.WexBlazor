# Xbim.WexBlazor

A Blazor component library that wraps the @xbim/viewer JavaScript library for use in Blazor WebAssembly applications. This library allows you to display 3D building models in the wexBIM format.

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

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio or your preferred IDE
3. Build the solution
4. Run the sample project

```bash
dotnet build
dotnet run --project src/Xbim.WexBlazor.Sample/Xbim.WexBlazor.Sample.csproj
```

## Using Components

You can use the components from this library in your Blazor WebAssembly projects by adding a reference to the Xbim.WexBlazor project:

```bash
dotnet add reference path/to/Xbim.WexBlazor.csproj
```

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

## License

MIT

## Acknowledgements

This project uses the [@xbim/viewer](https://www.npmjs.com/package/@xbim/viewer) JavaScript library from the [xBimTeam](https://github.com/xBimTeam/XbimWebUI). 
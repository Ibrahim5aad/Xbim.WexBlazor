# Xbim.WexBlazor TypeScript Implementation

This project uses TypeScript for JavaScript interop functionality to provide better type safety and development experience. The TypeScript files are automatically compiled during the build process using MSBuild's TypeScript compiler.

## TypeScript Integration

### How it Works

The TypeScript compilation is fully integrated into the MSBuild process:

1. TypeScript files are located in the `wwwroot/ts` directory
2. When you build the project with `dotnet build`, MSBuild automatically compiles TypeScript to JavaScript
3. The compiled JavaScript files are placed in the `wwwroot/js` directory
4. Blazor loads these JavaScript files at runtime

### No External Dependencies

This setup does not require Node.js or npm to be installed. The TypeScript compiler is provided by the `Microsoft.TypeScript.MSBuild` NuGet package, which is included in the project.

## Adding External Libraries

Since this project doesn't use npm, external JavaScript libraries are added using Microsoft's Library Manager (LibMan), which is fully integrated into the build process:

### Automatic Library Restoration

The project uses the `Microsoft.Web.LibraryManager.Build` NuGet package to automatically restore client-side libraries during the build process. When you build the project with `dotnet build`:

1. MSBuild automatically calls LibMan to restore any libraries defined in `libman.json`
2. The libraries are downloaded and placed in the specified destination folders
3. No manual commands needed

### Managing Libraries with LibMan

To add or update libraries:

1. Edit the `libman.json` file in the project root:
   ```json
   {
     "version": "1.0",
     "defaultProvider": "cdnjs",
     "libraries": [
       {
         "library": "@xbim/viewer@latest",
         "destination": "wwwroot/lib/xbim-viewer",
         "provider": "jsdelivr"
       }
     ]
   }
   ```

2. Build the project to restore the libraries:
   ```bash
   dotnet build
   ```

3. Reference the libraries in your TypeScript code:
   ```typescript
   const script = document.createElement('script');
   script.src = '_content/Xbim.WexBlazor/lib/xbim-viewer/index.js';
   ```

### LibMan Configuration

- **library**: The name and version of the library
- **destination**: Where to place the files
- **provider**: Source provider (jsdelivr, cdnjs, unpkg, etc.)
- **files**: Optional - specific files to include (when omitted, all files are included)

### Alternatives to LibMan

1. **Manual Download**: Download library files directly and place them in `wwwroot/lib/`
2. **CDN References**: Add script tags in `index.html` with CDN URLs
3. **NuGet Packages**: Some libraries are available as NuGet packages with static assets

## TypeScript Files Structure

- `wwwroot/ts/xbimViewerInterop.ts` - Main interop functionality with the xBIM viewer
- `tsconfig.json` - TypeScript compiler configuration

These files are compiled to JavaScript in the `wwwroot/js` directory, which is what Blazor loads at runtime.

## Adding New Functionality

1. Add or modify TypeScript files in the `wwwroot/ts` directory
2. Add corresponding C# methods in `Interop/XbimViewerInterop.cs`
3. Add component methods in `Components/XbimViewerComponent.razor`
4. Build the project with `dotnet build` to generate the JavaScript files

## Important Files

- `Interop/XbimViewerInterop.cs` - C# interop class that calls into JavaScript
- `ModulePath = "./_content/Xbim.WexBlazor/js/xbimViewerInterop.js"` - The path where Blazor looks for the compiled JavaScript
- `Components/XbimViewerComponent.razor` - The Blazor component that uses the interop

## TypeScript Configuration

TypeScript compilation is configured in two places:

1. **Project file (Xbim.WexBlazor.csproj)** - Contains MSBuild settings for TypeScript compilation
2. **tsconfig.json** - Contains TypeScript compiler options

If you need to modify TypeScript compiler options, you can update either or both of these files. 
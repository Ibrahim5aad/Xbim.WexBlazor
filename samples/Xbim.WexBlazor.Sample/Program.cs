using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Xbim.WexBlazor;
using Xbim.WexBlazor.Sample;
using Xbim.WexBlazor.Services;
using Xbim.WexBlazor.Services.Abstractions;
using Xbim.WexBlazor.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient for static asset loading
var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
builder.Services.AddScoped(_ => httpClient);

// Add Xbim.WexBlazor standalone services with configuration
builder.Services.AddWexBlazorStandalone(options =>
{
    options.InitialTheme = ViewerTheme.Dark;
    options.LightAccentColor = "#0969da";
    options.DarkAccentColor = "#1e7e34";
    options.LightBackgroundColor = "#ffffff";
    options.DarkBackgroundColor = "#404040";

    // Configure FileLoaderPanel with demo models from wwwroot
    options.FileLoaderPanel
        .AddDemoModel("Sample House", "models/SampleHouse.wexbim")
        .AddDemoModel("Four Walls", "models/FourWalls.wexbim");
});

// Build the host first to access services
var host = builder.Build();

// Add a custom property source that simulates fetching from an API
var propertyService = host.Services.GetRequiredService<IPropertyService>();
var apiPropertySource = new CustomPropertySource(
    async (query, ct) =>
    {
        // Simulate API latency
        await Task.Delay(100, ct);

        // Generate mock properties based on element ID
        var props = new ElementProperties
        {
            ElementId = query.ElementId,
            ModelId = query.ModelId,
            Name = $"Element #{query.ElementId}",
            TypeName = GetMockTypeName(query.ElementId)
        };

        // Add simulated API data group
        props.Groups.Add(new PropertyGroup
        {
            Name = "API Data",
            Source = "REST API",
            Properties = new List<PropertyValue>
            {
                new() { Name = "Element ID", Value = query.ElementId.ToString(), ValueType = "integer" },
                new() { Name = "Model ID", Value = query.ModelId.ToString(), ValueType = "integer" },
                new() { Name = "Last Updated", Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"), ValueType = "datetime" },
                new() { Name = "Status", Value = GetMockStatus(query.ElementId), ValueType = "string" },
                new() { Name = "Priority", Value = GetMockPriority(query.ElementId), ValueType = "string" }
            }
        });

        // Add simulated maintenance data
        props.Groups.Add(new PropertyGroup
        {
            Name = "Maintenance",
            Source = "REST API",
            Properties = new List<PropertyValue>
            {
                new() { Name = "Last Inspection", Value = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 365)).ToString("yyyy-MM-dd"), ValueType = "date" },
                new() { Name = "Next Scheduled", Value = DateTime.UtcNow.AddDays(Random.Shared.Next(30, 180)).ToString("yyyy-MM-dd"), ValueType = "date" },
                new() { Name = "Condition Score", Value = (Random.Shared.NextDouble() * 4 + 1).ToString("F1"), ValueType = "decimal" },
                new() { Name = "Warranty Expires", Value = DateTime.UtcNow.AddYears(Random.Shared.Next(1, 5)).ToString("yyyy-MM-dd"), ValueType = "date" }
            }
        });

        return props;
    },
    sourceType: "REST API",
    name: "Simulated API Properties"
);
propertyService.RegisterSource(apiPropertySource);

await host.RunAsync();

// Helper functions for mock data
static string GetMockTypeName(int id) => (id % 5) switch
{
    0 => "IfcWall",
    1 => "IfcSlab",
    2 => "IfcDoor",
    3 => "IfcWindow",
    _ => "IfcBuildingElementProxy"
};

static string GetMockStatus(int id) => (id % 4) switch
{
    0 => "Active",
    1 => "Pending Review",
    2 => "Approved",
    _ => "In Progress"
};

static string GetMockPriority(int id) => (id % 3) switch
{
    0 => "High",
    1 => "Medium",
    _ => "Low"
};

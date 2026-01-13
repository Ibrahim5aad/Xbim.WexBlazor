using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Xbim.WexBlazor.Sample;
using Xbim.WexBlazor.Services;
using Xbim.WexBlazor.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var themeService = new ThemeService();
themeService.SetTheme(ViewerTheme.Dark);
themeService.SetAccentColors(
    lightColor: "#0969da",
    darkColor: "#1e7e34"
);
builder.Services.AddSingleton(themeService);

await builder.Build().RunAsync();

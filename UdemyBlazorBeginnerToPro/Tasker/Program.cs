using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tasker.Components;
using Tasker.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();

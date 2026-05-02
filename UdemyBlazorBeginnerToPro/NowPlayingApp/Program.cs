using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using NowPlayingApp.Services;

namespace NowPlayingApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
            });

            builder.Services.Configure<TMDBClientSettings>(
                builder.Configuration.GetSection("TMDBClientSettings")
            );

            builder
                .Services.AddHttpClient<TMDBClient>(
                    (sp, client) =>
                    {
                        var settings = sp.GetRequiredService<IOptions<TMDBClientSettings>>();
                        ValidateSettings(settings.Value);

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Bearer",
                            settings.Value.TMDBAccessKey
                        );

                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.BaseAddress = new Uri(settings.Value.TMDBApiBaseAddress!);
                    }
                )
                .AddStandardResilienceHandler();

            builder.Services.AddScoped<LocalStorageService>();
            builder.Services.AddScoped<IFavoritesService, FavoritesService>();

            await builder.Build().RunAsync();
        }

        private static void ValidateSettings(TMDBClientSettings settings)
        {
            ArgumentException.ThrowIfNullOrEmpty(settings.TMDBImageBaseAddress);
            ArgumentException.ThrowIfNullOrEmpty(settings.TMDBAccessKey);
            ArgumentException.ThrowIfNullOrEmpty(settings.TMDBApiBaseAddress);
        }
    }

    public sealed class TMDBClientSettings
    {
        public string? TMDBApiBaseAddress { get; set; }
        public string? TMDBImageBaseAddress { get; set; }
        public string? TMDBAccessKey { get; set; }
    }
}

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using NowPlayingApp.Services;

namespace NowPlayingApp
{
    /// <summary>
    /// Application entry point for the NowPlayingApp Blazor WebAssembly client.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configures services and starts the Blazor WebAssembly host.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the app.</param>
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

                        // When deployed behind a proxy (e.g. Netlify edge function), the proxy
                        // injects the Authorization header server-side, so no key is needed here.
                        // In local development the key comes from user secrets.
                        if (!string.IsNullOrEmpty(settings.Value.TMDBAccessKey))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                "Bearer",
                                settings.Value.TMDBAccessKey
                            );
                        }

                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        // A relative address (e.g. "tmdb/") means requests are routed through the
                        // host-origin proxy. An absolute address targets the TMDB API directly.
                        var apiBase = settings.Value.TMDBApiBaseAddress!;
                        client.BaseAddress = Uri.IsWellFormedUriString(apiBase, UriKind.Absolute)
                            ? new Uri(apiBase)
                            : new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBase);
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
            ArgumentException.ThrowIfNullOrEmpty(settings.TMDBApiBaseAddress);
        }
    }

    /// <summary>
    /// Strongly typed configuration settings used by <see cref="Services.TMDBClient"/>.
    /// </summary>
    public sealed class TMDBClientSettings
    {
        /// <summary>
        /// Gets or sets the TMDB API base address used for REST requests.
        /// </summary>
        public string? TMDBApiBaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the TMDB image CDN base address used to compose poster URLs.
        /// </summary>
        public string? TMDBImageBaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the TMDB bearer token used for API authentication.
        /// </summary>
        public string? TMDBAccessKey { get; set; }
    }
}

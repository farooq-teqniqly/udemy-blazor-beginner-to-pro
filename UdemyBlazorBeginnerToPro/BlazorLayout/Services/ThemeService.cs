using Microsoft.JSInterop;

namespace BlazorLayout.Services
{
    internal sealed class ThemeService
    {
        private readonly IJSRuntime _js;

        public static List<string> Themes { get; } =
            [
                "code-magic",
                "blue",
                "indigo",
                "purple",
                "pink",
                "red",
                "orange",
                "yellow",
                "green",
                "teal",
                "cyan",
                "gray",
            ];

        public static List<string> Weights { get; } =
            ["core", "100", "150", "200", "300", "400", "500", "600", "700", "800", "850", "900"];

        public ThemeService(IJSRuntime js)
        {
            ArgumentNullException.ThrowIfNull(js);
            _js = js;
        }

        public async Task ChangeThemeAsync(string theme)
        {
            ArgumentException.ThrowIfNullOrEmpty(theme);

            if (!Themes.Contains(theme))
            {
                throw new ArgumentException($"Invalid theme {theme}", nameof(theme));
            }

            await _js.InvokeVoidAsync("setTheme", theme);
        }
    }
}

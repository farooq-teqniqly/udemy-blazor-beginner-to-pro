using System.Text.Json;
using Microsoft.JSInterop;

namespace Tasker.Services
{
    public sealed class LocalStorageService
    {
        private readonly IJSRuntime _js;

        public LocalStorageService(IJSRuntime js)
        {
            ArgumentNullException.ThrowIfNull(js);

            _js = js;
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            var json = await _js.InvokeAsync<string>("localStorage.getItem", key);

            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentNullException.ThrowIfNull(value);

            var json = JsonSerializer.Serialize(value);
            await _js.InvokeVoidAsync("localStorage.setItem", key, json);
        }
    }
}

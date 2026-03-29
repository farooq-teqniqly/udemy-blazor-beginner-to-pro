using System.Text.Json;
using Microsoft.JSInterop;

namespace Tasker.Services
{
    /// <summary>
    /// Provides access to the browser's localStorage API for storing and retrieving data.
    /// </summary>
    public sealed class LocalStorageService
    {
        private readonly IJSRuntime _js;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalStorageService"/> class.
        /// </summary>
        /// <param name="js">The JavaScript runtime for interop with browser APIs.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="js"/> is null.</exception>
        public LocalStorageService(IJSRuntime js)
        {
            ArgumentNullException.ThrowIfNull(js);

            _js = js;
        }

        /// <summary>
        /// Retrieves an item from localStorage and deserializes it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the stored value into.</typeparam>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the deserialized value,
        /// or the default value of <typeparamref name="T"/> if the key doesn't exist or the value is empty.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or empty.</exception>
        public async Task<T?> GetItemAsync<T>(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            var json = await _js.InvokeAsync<string>("localStorage.getItem", key);

            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Serializes a value to JSON and stores it in localStorage under the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The key under which to store the value.</param>
        /// <param name="value">The value to serialize and store.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public async Task SetItemAsync<T>(string key, T value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentNullException.ThrowIfNull(value);

            var json = JsonSerializer.Serialize(value);
            await _js.InvokeVoidAsync("localStorage.setItem", key, json);
        }
    }
}

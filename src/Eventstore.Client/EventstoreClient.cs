using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eventstore.Client
{
    internal class EventstoreClient : IEventstoreClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        private JsonSerializerOptions _jsonSerializerOptions;

        public EventstoreClient(string baseUrl, HttpClient httpClient, JsonSerializerOptions jsonOptions)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
            _jsonSerializerOptions = jsonOptions;
        }

        async Task<EventStoreResult<T>> IEventstoreClient.Add<T>(string storename, string id, T toAdd)
        {
            if (string.IsNullOrEmpty(storename))
                throw new ArgumentException("storename can not be null or empty", nameof(storename));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id can not be null or empty", nameof(id));
            
            var ety = new EventstoreEntity<T>
            {
                id = id,
                version = 0,
                data = toAdd
            };

            var url = $"{_baseUrl}{storename}/entities/{id}";
            var json = JsonSerializer.Serialize(ety, _jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(url, content);

            if (result.StatusCode != HttpStatusCode.Created)
            {
                return new EventStoreResult<T>
                {
                    StatusCode = (int)result.StatusCode,
                    Resource = default(T)
                };
            }

            json = await result.Content.ReadAsStringAsync();
            ety = JsonSerializer.Deserialize<EventstoreEntity<T>>(json, _jsonSerializerOptions);

            return new EventStoreResult<T> 
            {
                StatusCode = (int)result.StatusCode,
                Resource = ety.data,
                Version = ety.version
            };
        }

        async Task<EventStoreResult<T>> IEventstoreClient.Get<T>(string storename, string id)
        {
            if (string.IsNullOrEmpty(storename))
                throw new ArgumentException("storename can not be null or empty", nameof(storename));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id can not be null or empty", nameof(id));

            var url = $"{_baseUrl}{storename}/entities/{id}";
            var result = await _httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                return new EventStoreResult<T>
                {
                    StatusCode = (int)result.StatusCode,
                    Resource = default(T)
                };
            }

            var json = await result.Content.ReadAsStringAsync();
            var ety = JsonSerializer.Deserialize<EventstoreEntity<T>>(json, _jsonSerializerOptions);

            return new EventStoreResult<T>
            {
                StatusCode = (int)result.StatusCode,
                Resource = ety.data,
                Version = ety.version
            };
        }

        async Task<EventStoreResult<T>> IEventstoreClient.GetByVersion<T>(string storename, string id, int version)
        {
            if (string.IsNullOrEmpty(storename))
                throw new ArgumentException("storename can not be null or empty", nameof(storename));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id can not be null or empty", nameof(id));

            var url = $"{_baseUrl}{storename}/entities/{id}?version={version}";
            var result = await _httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                return new EventStoreResult<T>
                {
                    StatusCode = (int)result.StatusCode,
                    Resource = default(T)
                };
            }

            var json = await result.Content.ReadAsStringAsync();
            var ety = JsonSerializer.Deserialize<EventstoreEntity<T>>(json, _jsonSerializerOptions);

            return new EventStoreResult<T>
            {
                StatusCode = (int)result.StatusCode,
                Resource = ety.data,
                Version = ety.version
            };
        }

        async Task<EventStoreResult<T>> IEventstoreClient.Save<T>(string storename, string id, T toSave)
        {
            if (string.IsNullOrEmpty(storename))
                throw new ArgumentException("storename can not be null or empty", nameof(storename));
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id can not be null or empty", nameof(id));
            
            var ety = new EventstoreEntity<T>
            {
                id = id,
                version = 0,
                data = toSave
            };

            var url = $"{_baseUrl}{storename}/entities/{id}";
            var json = JsonSerializer.Serialize(ety, _jsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await _httpClient.PutAsync(url, content);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                return new EventStoreResult<T>
                {
                    StatusCode = (int)result.StatusCode,
                    Resource = default(T)
                };
            }

            json = await result.Content.ReadAsStringAsync();
            ety = JsonSerializer.Deserialize<EventstoreEntity<T>>(json, _jsonSerializerOptions);

            return new EventStoreResult<T> 
            {
                StatusCode = (int)result.StatusCode,
                Resource = ety.data,
                Version = ety.version
            };
        }
    }
}
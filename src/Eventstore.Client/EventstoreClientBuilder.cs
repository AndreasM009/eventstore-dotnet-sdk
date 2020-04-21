using System;
using System.Net.Http;
using System.Text.Json;

namespace Eventstore.Client
{
    public class EventstoreClientBuilder
    {
        private string _endpoint;
        private Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient());
        private JsonSerializerOptions _jsonSerializerOptions;

        public EventstoreClientBuilder()
        {
            var port = System.Environment.GetEnvironmentVariable("EVENTSTORE_HTTP_PORT") ?? "5600";
            _endpoint = $"http://127.0.0.1:{port}/eventstores/";
        }

        /// <summary>
        /// Overrides the default endpoint used by IEventstoreClient to connect to the eventstore runtime.
        /// </summary>
        /// <param name="endpoint">Endpoint to use for making calls to Eventstore runtime. 
        /// Default endpoint used is http://127.0.0.1:5600.</param>
        /// <returns>EventstoreClientBuilder</returns>
        public EventstoreClientBuilder UseEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException();

            this._endpoint = endpoint + "/eventstores/";
            return this;
        }

        public EventstoreClientBuilder UseJsonSerializationOptions(JsonSerializerOptions options)
        {
            _jsonSerializerOptions = options;
            return this;
        }

        public EventstoreClientBuilder UseHttpClient(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = new Lazy<HttpClient>(() => httpClient);
            return this;
        }

        public IEventstoreClient Build()
        {
            return new EventstoreClient(_endpoint, _httpClient.Value, _jsonSerializerOptions);
        }
    }
}
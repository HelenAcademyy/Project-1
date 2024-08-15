using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Helen.Domain;
using Helen.Domain.GenericResponse;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Helen.Service
{
    public interface IHttpService
    {
        Task<GenericHttpResponse<T>> GetRequest<T>(string url) where T : class;
        Task<GenericHttpResponse<T>> PostRequest<T>(string url, object payload) where T : class;
        Task<GenericHttpResponse<T>> GetToken<T>() where T : class;
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HttpService> _logger;
        private readonly IMemoryCache _cache;

        public HttpService(HttpClient client, IConfiguration configuration, ILogger<HttpService> logger, IMemoryCache cache)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<GenericHttpResponse<T>> GetRequest<T>(string url) where T : class
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new GenericHttpResponse<T>();

            try
            {
                _logger.LogInformation("Sending GET request to {Url}", url);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                };

                using var httpResponse = await _client.SendAsync(request);
                stopwatch.Stop();

                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                response.ResponseCode = httpResponse.StatusCode.ToString();
                response.Content = responseBody;

                if (httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("GET request to {Url} succeeded in {ElapsedTime} ms", url, stopwatch.ElapsedMilliseconds);
                    response.ResponseObject = JsonSerializer.Deserialize<T>(responseBody);
                }
                else
                {
                    _logger.LogWarning("GET request to {Url} failed with status code {StatusCode} in {ElapsedTime} ms", url, httpResponse.StatusCode, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while making GET request to {Url}", url);

            }

            return response;
        }

        public async Task<GenericHttpResponse<T>> PostRequest<T>(string url, object payload) where T : class
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new GenericHttpResponse<T>();

            try
            {
                _logger.LogInformation("Sending POST request to {Url} with payload {Payload}", url, payload);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
                };

                using var httpResponse = await _client.SendAsync(request);
                stopwatch.Stop();

                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                response.ResponseCode = httpResponse.StatusCode.ToString();
                response.Content = responseBody;

                if (httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("POST request to {Url} succeeded in {ElapsedTime} ms", url, stopwatch.ElapsedMilliseconds);
                    response.ResponseObject = JsonSerializer.Deserialize<T>(responseBody);
                }
                else
                {
                    _logger.LogWarning("POST request to {Url} failed with status code {StatusCode} in {ElapsedTime} ms", url, httpResponse.StatusCode, stopwatch.ElapsedMilliseconds);
                   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while making POST request to {Url}", url);

            }

            return response;
        }

        public async Task<GenericHttpResponse<T>> GetToken<T>() where T : class
        {
            const string cacheKey = "Token";
            var absoluteExpirationMinutes = int.Parse(_configuration["Cache:AbsoluteExpirationMinutes"]);
            var slidingExpirationMinutes = int.Parse(_configuration["Cache:SlidingExpirationMinutes"]);

            if (!_cache.TryGetValue(cacheKey, out GenericHttpResponse<T> cachedToken))
            {
                var stopwatch = Stopwatch.StartNew();
                string url = _configuration["MTN:Token"];
                var response = await GetRequest<T>(url);
                stopwatch.Stop();

                if (response.ResponseObject != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationMinutes),
                        SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationMinutes)
                    };
                    _cache.Set(cacheKey, response.ResponseObject, cacheEntryOptions);

                    _logger.LogInformation("Token request to {Url} succeeded in {ElapsedTime} ms", url, stopwatch.ElapsedMilliseconds);
                    return response;
                }
                else
                {
                    _logger.LogWarning("Received empty token response in {ElapsedTime} ms", stopwatch.ElapsedMilliseconds);
                }
            }

            return new GenericHttpResponse<T> { ResponseObject = cachedToken.ResponseObject };
        }
    }
}

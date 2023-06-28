using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Asoode.Shared.Abstraction.Contracts;

namespace Asoode.Shared.Core.Implementations;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly IJsonService _jsonService;
    private readonly ILoggerService _logger;
    private string _token = string.Empty;

    public HttpService(ILoggerService logger, IJsonService jsonService)
    {
        _logger = logger;
        _jsonService = jsonService;
        _httpClient = new HttpClient();
    }

    public void ConfigureToken(string token, string? schema = null)
    {
        if (token != _token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(schema ?? "Bearer", _token);
        }
    }

    public async Task<T?> Get<T>(string path, Dictionary<string, string> parameters)
    {
        try
        {
            var url = AddQueryString(path, parameters);
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T?>();
            return default;
        }
        catch (Exception e)
        {
            await _logger.Error(e.Message, "HttpService.Get", e);
            return default;
        }
    }

    public void ConfigureEndpoint(string endpoint)
    {
        _httpClient.BaseAddress = new Uri(endpoint);
    }

    public async Task<string?> Submit(string path, object? payload)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(path, payload ?? new { });
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return content;
            throw new Exception(content);
        }
        catch (Exception e)
        {
            await _logger.Error(e.Message, "HttpService.Post", e);
            return default;
        }
    }

    public async Task<T?> Post<T>(string path, object? payload)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(path, payload ?? new { });
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T?>();
            return default;
        }
        catch (Exception e)
        {
            await _logger.Error(e.Message, "HttpService.Post", e);
            return default;
        }
    }

    public async Task<T?> Get<T>(string path)
    {
        try
        {
            var response = await _httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T?>();
            return default;
        }
        catch (Exception e)
        {
            await _logger.Error(e.Message, "HttpService.Get", e);
            return default;
        }
    }

    private static string AddQueryString(string url, Dictionary<string, string> parameters)
    {
        var builder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(builder.Query);

        foreach (var parameter in parameters)
            query[parameter.Key] = parameter.Value;

        builder.Query = query.ToString();
        return builder.ToString();
    }
}
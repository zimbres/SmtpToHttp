namespace SmtpToHttp.Services;

public class HttpService
{
    private readonly ILogger<HttpService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public HttpService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<HttpService> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
    }
    public async Task PostAsync(MailMessage message)
    {
        var configuration = _configuration.GetSection("HttpConfiguration").Get<HttpConfiguration>();

        try
        {
            await _httpClient.PostAsJsonAsync(configuration.Url, message);
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex.Message);
        }
    }
}

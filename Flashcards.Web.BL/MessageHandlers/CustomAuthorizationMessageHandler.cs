using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Logging;

namespace Flashcards.Web.BL.MessageHandlers;
public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    private readonly IAccessTokenProvider _provider;
    private readonly ILogger<CustomAuthorizationMessageHandler> _logger;
    
    public CustomAuthorizationMessageHandler(
        ILogger<CustomAuthorizationMessageHandler> logger,
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration) 
        : base(provider, navigation)
    {
        _provider = provider;
        _logger = logger;
        
        ConfigureHandler(
            authorizedUrls: new[] { configuration["ApiOptions:BaseUrl"] },
            scopes: new[] { "flashcardsapi" }
        );
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        var accessTokenResult = await _provider.RequestAccessToken();

        if (accessTokenResult.TryGetToken(out var token))
        {
            _logger.LogInformation("Access token found. Expires: '{AcccessTokenExpires}'. Sending request to {Url}", 
                token.Expires, request.RequestUri);
            
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            _logger.LogWarning("No access token found. Sending anonymous request to {Url}", request.RequestUri);

            // User is not logged in, bypass authorization by not calling base implementation
            using var httpClient = new HttpClient();

            // Copy the request to avoid disposal issues
            using var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            // Copy headers (except authorization-related ones)
            foreach (var header in request.Headers.Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Copy content if present
            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                newRequest.Content = new ByteArrayContent(contentBytes);

                // Copy content headers
                foreach (var header in request.Content.Headers)
                {
                    newRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return await httpClient.SendAsync(newRequest, cancellationToken);
        }
    }
}
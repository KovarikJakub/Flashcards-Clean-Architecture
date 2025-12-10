using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Flashcards.Api.BL.Services;

public class IdentityServerClient
{
    private readonly HttpClient _httpClient;
    private readonly IdentityServerOptions _options;
    private TokenResponse? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public IdentityServerClient(HttpClient httpClient, IOptions<IdentityServerOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Gets a valid access token, using cache if available
    /// </summary>
    private async Task<string> GetAccessTokenAsync()
    {
        // Check if we have a cached token that's still valid
        if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken.AccessToken!;
        }

        // Request new token using client credentials
        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = $"{_options.Authority}/connect/token",
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scope = "flashcardsapi"
            });

        if (tokenResponse.IsError)
        {
            throw new InvalidOperationException(
                $"Failed to get access token from Identity Server: {tokenResponse.Error} - {tokenResponse.ErrorDescription}");
        }

        // Cache the token (with 1 minute buffer before expiry)
        _cachedToken = tokenResponse;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);

        return tokenResponse.AccessToken!;
    }

    /// <summary>
    /// Creates a new user in the Identity Server
    /// </summary>
    public async Task<Guid> CreateUserAsync(CreateUserRequest request)
    {
        var token = await GetAccessTokenAsync();
        _httpClient.SetBearerToken(token);

        var response = await _httpClient.PostAsJsonAsync(
            $"{_options.Authority}/api/user",
            new
            {
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                Subject = request.UserName,
                Role = request.Role
            });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Failed to create user in Identity Server: {response.StatusCode} - {error}");
        }

        var userId = await response.Content.ReadFromJsonAsync<Guid>();
        return userId;
    }

    /// <summary>
    /// Deletes a user from the Identity Server (if endpoint exists)
    /// </summary>
    public async Task DeleteUserAsync(Guid userId)
    {
        var token = await GetAccessTokenAsync();
        _httpClient.SetBearerToken(token);

        var response = await _httpClient.DeleteAsync(
            $"{_options.Authority}/api/user/{userId}");

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Failed to delete user in Identity Server: {response.StatusCode} - {error}");
        }
    }
}

public class CreateUserRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}

public class IdentityServerOptions
{
    public required string Authority { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}

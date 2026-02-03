using Flashcards.Api.BL.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;

namespace Flashcards.Api.BL.Services
{
    public class DuendeIdentityService(HttpClient httpClient, IOptions<IdentityServerOptions> options)
        : IIdentityService
    {
        private TokenResponse? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;
        private readonly IdentityServerOptions _options = options.Value;

        private async Task<string> GetAccessTokenAsync()
        {
            // Check if we have a cached token that's still valid
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken.AccessToken!;
            }

            // Request new token using client credentials
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = $"{_options.Authority}/connect/token",
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret,
                    Scope = _options.Scope
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

        public async Task<string> CreateIdentityAsync(string username, string password, string role)
        {
            // Prepare the payload required by your Duende Controller
            var requestBody = new
            {
                Username = username, 
                Password = password,
                Subject = username,
                Email = $"{username}@email.com",
                Role = role
            };

            // Call the Duende Endpoint
            var response = await httpClient.PostAsJsonAsync($"{_options.Authority}/api/user", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Identity Creation Failed: {response.StatusCode} - {errorDetails}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return result.Trim('"');
        }

        public async Task DeleteIdentityAsync(string userId)
        {

            var response = await httpClient.DeleteAsync($"{_options.Authority}/api/user/{userId}");

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Failed to delete user in Identity Server: {response.StatusCode} - {error}");
            }
        }
    }

    public class IdentityServerOptions
    {
        public required string Authority { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Scope { get; set; }
    }
}

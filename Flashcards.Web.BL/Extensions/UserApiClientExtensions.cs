using Flashcards.Common.Enums;
using Flashcards.Web.BL.Api;
using System.Net.Http.Json;

namespace Flashcards.Web.BL.Extensions;

/// <summary>
/// Extension methods for IUserApiClient to support operations not covered by generated API client
/// </summary>
public static class UserApiClientExtensions
{
    /// <summary>
    /// Creates a new user in both Identity Server and API using the authenticated HttpClient
    /// </summary>
    public static async Task<Guid> CreateUserWithIdentityAsync(
        this IUserApiClient client,
        HttpClient httpClient,
        string userName,
        string password,
        string email,
        string name,
        string? photographyUrl,
        RoleType role,
        CancellationToken cancellationToken = default)
    {
        var request = new CreateUserWithIdentityRequest
        {
            UserName = userName,
            Password = password,
            Email = email,
            Name = name,
            PhotographyUrl = photographyUrl,
            Role = role
        };

        var response = await httpClient.PostAsJsonAsync(
            "api/user/create-with-identity", 
            request, 
            cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var userId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
        return userId;
    }

    /// <summary>
    /// Deletes user from both Identity Server and API
    /// </summary>
    public static async Task DeleteUserWithIdentityAsync(
        this IUserApiClient client,
        HttpClient httpClient,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(
            $"api/user/{userId}/with-identity", 
            cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Request model for creating user with identity
    /// </summary>
    private class CreateUserWithIdentityRequest
    {
        public required string UserName { get; init; }
        public required string Password { get; init; }
        public required string Email { get; init; }
        public required string Name { get; init; }
        public string? PhotographyUrl { get; init; }
        public required RoleType Role { get; init; }
    }
}

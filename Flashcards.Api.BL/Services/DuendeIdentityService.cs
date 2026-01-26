using System.Net.Http.Json;

namespace Flashcards.Api.BL.Services
{
    public class DuendeIdentityService(HttpClient httpClient) : IIdentityService
    {
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
            var response = await httpClient.PostAsJsonAsync("api/user", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Identity Creation Failed: {response.StatusCode} - {errorDetails}");
            }

            var result = await response.Content.ReadFromJsonAsync<IdentityCreationResult>();
            return result!.UserId;
        }

        public async Task DeleteIdentityAsync(string userId)
        {
            var response = await httpClient.DeleteAsync($"api/user/{userId}");
        }

        private class IdentityCreationResult
        {
            public string UserId { get; set; }
        }
    }
}

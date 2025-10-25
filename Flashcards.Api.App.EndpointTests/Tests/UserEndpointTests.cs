using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Common.Models.CardCollection;
using Flashcards.Common.Models.Lesson;
using Flashcards.Common.Models.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Flashcards.Common.Enums;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class UserEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly TestWebAppFactory _app;
        private readonly Lazy<HttpClient> _client;
        private readonly UserMapper _userMapper = new UserMapper();
        private readonly CardCollectionMapper _collectionMapper = new CardCollectionMapper();
        
        public UserEndpointsTests()
        {
            _app = new TestWebAppFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
        }

        private IStorage GetStorageInstance() => _app.Services.GetRequiredService<IStorage>();

        [Fact]
        public async Task Get_On_Seeded_Storage_Returns_Not_Empty()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();
            
            // Act
            var result = await _client.Value.GetAsync("/api/user/all");
            
            // Assert
            result.EnsureSuccessStatusCode();

            var users = await result.Content.ReadFromJsonAsync<ICollection<UserListModel>>();
            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task Get_On_Empty_Seed_Returns_None()
        {
            // Arrange
            var storage = GetStorageInstance();
            
            // Act
            var result = await _client.Value.GetAsync("/api/user/all");
            
            // Assert
            result.EnsureSuccessStatusCode();

            var users = await result.Content.ReadFromJsonAsync<ICollection<UserListModel>>();
            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task Delete_Existing_User()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            var existed_user = storage.Users[0];
            
            // Act
            var result = await _client.Value.DeleteAsync($"api/user/{existed_user.Id}");
            
            // Assert
            result.EnsureSuccessStatusCode();
            Assert.DoesNotContain(storage.Users, user => user.Id == existed_user.Id);

        }

        [Fact]
        public async Task Post_New_Entity_Add()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            var newUser = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Igor",
                PhotographyUrl = null,
                Role = RoleType.Administrator
            };

            var json = JsonSerializer.Serialize(newUser);
            var context = new StringContent(json, Encoding.UTF8, "application/json");
            // Act
            var result = await _client.Value.PostAsync("api/user", context);
            
            // Assert
            result.EnsureSuccessStatusCode();
            Assert.Contains(storage.Users, user => user.Id == newUser.Id);
        }
    }
}


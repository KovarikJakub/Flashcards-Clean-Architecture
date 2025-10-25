using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Common.Models.Card;
using Flashcards.Common.Models.CardCollection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class CardCollectionEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly TestWebAppFactory _app;
        private readonly Lazy<HttpClient> _client;
        private readonly CardCollectionMapper _collectionMapper = new CardCollectionMapper();

        public CardCollectionEndpointsTests()
        {
            _app = new TestWebAppFactory();
            _client = new Lazy<HttpClient>(_app.CreateClient());
        }

        private IStorage GetStorageInstance() => _app.Services.GetRequiredService<IStorage>();

        [Fact]
        public async Task Get_OnEmptyStorage_Returns_None()
        {
            // Arrange
            var storage = GetStorageInstance();

            // Act
            var response = await _client.Value.GetAsync("/api/cardcollection/all");

            // Assert
            response.EnsureSuccessStatusCode();

            var collections = await response.Content.ReadFromJsonAsync<ICollection<CardCollectionListModel>>();
            Assert.NotNull(collections);
            Assert.Empty(collections);
        }

        [Fact]
        public async Task Get_OnSeededStorage_Returns_NotEmpty()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            // Act
            var response = await _client.Value.GetAsync("/api/cardcollection/all");

            // Assert
            response.EnsureSuccessStatusCode();

            var collections = await response.Content.ReadFromJsonAsync<ICollection<CardCollectionListModel>>();
            Assert.NotNull(collections);
            Assert.NotEmpty(collections);
        }

        [Fact]
        public async Task Delete_ExistingEntity_RemovesEntity()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            var existingEntity = storage.Collections[0];

            // Act
            var response = await _client.Value.DeleteAsync($"/api/cardcollection/{existingEntity.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(storage.Collections.Any(c => c.Id == existingEntity.Id));
        }

        [Fact]
        public async Task Post_NewEntity_AddsEntity()
        {
            // Arrange
            var storage = GetStorageInstance();
            storage.Seed();

            var newCollection = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = "History Facts",
                AnswerStart = DateTime.UtcNow.AddMinutes(-10),
                AnswerEnd = DateTime.UtcNow.AddMinutes(10),
                Cards = new List<CardDetailModel>()
            };

            var json = JsonSerializer.Serialize(newCollection);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.Value.PostAsync("/api/cardcollection", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(storage.Collections.Any(c => c.Id == newCollection.Id));
        }
    }
}

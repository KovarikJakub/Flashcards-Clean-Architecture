using Flashcards.Api.BL.Mappers;
using Flashcards.Common.Models.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Enums;

namespace Flashcards.Api.App.EndpointTests.Tests
{
    public class UserEndpointsTests : BaseEndpointTests<UserEntity, UserDetailModel, UserListModel>, IClassFixture<WebApplicationFactory<Program>>
    {
        private static readonly UserMapper UserMapper = new();

        protected override string EndpointBase => "/api/user";
        protected override string EndpointGetAll => "/api/user";
        protected override Guid GetEntityId(UserEntity entity) => entity.Id;
        protected override Guid GetModelId(UserDetailModel model) => model.Id;
        protected override IList<UserEntity> GetCollection() => _storage.Users;
        protected override UserDetailModel CreateNewModel() => new() // new model for testing insert
        {
            Id = Guid.NewGuid(),
            Name = "Igor",
            PhotographyUrl = null,
            Role = RoleType.Administrator
        };
        protected override UserDetailModel GetUpdatedModel(UserEntity entity)
        {
            var model = UserMapper.ToDetailModel(entity);
            model.Name = "Jine Jmeno nez to prechozi";
            return model;
        }

        protected override bool VerifyUpdated(UserDetailModel updatedModel)
            => GetCollection().Any(u => u.Id == updatedModel.Id && u.Name == updatedModel.Name);
    }
}


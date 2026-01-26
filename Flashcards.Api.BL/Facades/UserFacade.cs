using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class UserFacade(
        IUserRepository userRepository,
        IIdentityService identityService,
        UserMapper mapper) : IUserFacade
    {
        public List<UserListModel> GetAll()
        {
            var entities = userRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public List<UserListModel> GetByQuery(UserQueryModel query)
        {
            var collection = userRepository.GetAll().AsEnumerable();

            if (query.Role is not null)
            {
                collection = collection.Where(u => u.Role == query.Role);
            }

            collection = QueryHelper.PerformPagination(collection, query);

            return mapper.ToListModel(collection.ToList());
        }

        public UserDetailModel? GetById(Guid id)
        {
            var entity = userRepository.GetById(id);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public Guid Create(UserCreateModel model, IList<string> userRoles, string requestorId)
        {
            if (string.IsNullOrWhiteSpace(requestorId))
                throw new ArgumentException("ownerId is required to create user.", nameof(requestorId));

            var role = model.Role == RoleType.Admin ? "Admin" : "User";
            var subId = identityService.CreateIdentityAsync(model.IdentityName, model.Password, role).Result;
            if (string.IsNullOrWhiteSpace(requestorId))
                throw new InvalidOperationException("Failed to create identity for the user.");

            var entity = mapper.ToEntity(model, subId);
            userRepository.Insert(entity);
            return entity.Id;
        }

        public Guid? Update(UserDetailModel model, IList<string> userRoles, string requestorId)
        {
            var existing = userRepository.GetById(model.Id);
            if (existing == null)
                return null;

            var entity = mapper.ToEntity(model, requestorId);

            return userRepository.Update(entity);
        }

        public void Delete(Guid id, IList<string> userRoles, string requestorId)
        {
            var deletedUserEntity = userRepository.GetById(id);
            if (deletedUserEntity is null)
                return;

            identityService.DeleteIdentityAsync(deletedUserEntity.IdentityOwnerId);
            userRepository.Remove(id);
        }
    }
}
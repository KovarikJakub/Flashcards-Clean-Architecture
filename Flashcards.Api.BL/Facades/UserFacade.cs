using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserRepository userRepository;
        private readonly UserMapper mapper;

        public UserFacade(
            IUserRepository userRepository,
            UserMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public List<UserListModel> GetAll()
        {
            var entities = userRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public UserDetailModel? GetById(Guid id)
        {
            var entity = userRepository.GetById(id);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public Guid CreateOrUpdate(UserDetailModel model, IList<string> userRoles, string? ownerId = null)
        {
            ThrowIfWrongOwnerAndNotAdmin(model.Id, userRoles, ownerId);
            return userRepository.Exists(model.Id)
                ? Update(model, userRoles, ownerId)!.Value
                : Create(model, userRoles, ownerId);
        }

        public Guid Create(UserDetailModel model, IList<string> userRoles, string? ownerId = null)
        {
            var entity = mapper.ToEntity(model, ownerId);
            userRepository.Insert(entity);
            return entity.Id;
        }

        /// <summary>
        /// Creates both Identity user and API user in one operation
        /// </summary>
        public async Task<Guid> CreateUserWithIdentityAsync(
            string userName,
            string password,
            string email,
            string name,
            string? photographyUrl,
            RoleType role,
            IList<string> userRoles,
            string? ownerId = null)
        {
            throw new NotImplementedException("Identity Server integration is temporarily disabled.");
        }

        public Guid? Update(UserDetailModel model, IList<string> userRoles, string? ownerId = null)
        {
            ThrowIfWrongOwnerAndNotAdmin(model.Id, userRoles, ownerId);

            var existing = userRepository.GetById(model.Id);
            if (existing == null)
                return null;

            var entity = mapper.ToEntity(model, ownerId);


            return userRepository.Update(entity);
        }

        public void Delete(Guid id, IList<string> userRoles, string? ownerId = null)
        {
            ThrowIfWrongOwnerAndNotAdmin(id, userRoles, ownerId);
            userRepository.Remove(id);
        }

        /// <summary>
        /// Deletes both API user and Identity user
        /// </summary>
        public async Task DeleteUserWithIdentityAsync(Guid id, IList<string> userRoles, string? ownerId = null)
        {
            throw new NotImplementedException("Identity Server integration is temporarily disabled.");
        }

        public UserDetailModel? GetByOwnerId(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId)) return null;
            var userEntity = userRepository.GetAll().FirstOrDefault(u => u.OwnerId != null && u.OwnerId.Equals(ownerId, StringComparison.OrdinalIgnoreCase));
            return userEntity == null ? null : mapper.ToDetailModel(userEntity);
        }

        private void ThrowIfWrongOwnerAndNotAdmin(Guid? id, IList<string> userRoles, string? ownerId)
        {
            if (id is not null
                && userRoles is not null
                && !userRoles.Contains(RoleType.Admin.ToString())  // Changed from Administrator
                && ownerId is not null)
            {
                var existingUser = userRepository.GetById(id.Value);
                if (existingUser != null && existingUser.Id.ToString() != ownerId)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
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

        private void ThrowIfWrongOwnerAndNotAdmin(Guid? id, IList<string> userRoles, string? ownerId)
        {
            if (id is not null
                && userRoles is not null
                && !userRoles.Contains(RoleType.Administrator.ToString())
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
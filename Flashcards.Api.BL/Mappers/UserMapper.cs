using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Mappers
{
    public class UserMapper
    {
        public UserEntity ToEntity(UserDetailModel model)
        {
            return new UserEntity
            {
                Id = model.Id,
                Name = model.Name,
                PhotographyUrl = model.PhotographyUrl,
                Role = model.Role
            };
        }

        public UserDetailModel ToDetailModel(UserEntity model)
        {
            return new UserDetailModel
            {
                Id = model.Id,
                Name = model.Name,
                PhotographyUrl = model.PhotographyUrl,
                Role = model.Role
            };
        }

        public List<UserListModel> ToListModel(IEnumerable<UserEntity> entities)
        {
            return entities.Select(e => new UserListModel
            {
                Id = e.Id,
                Name = e.Name,
                Role = e.Role
            }).ToList();
        }
    }
}
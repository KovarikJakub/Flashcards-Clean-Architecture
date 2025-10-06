
using System.Data;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models.User;

namespace Flashcards.Api.BL.Facades
{

    public class UserFacade : IUserfacade
    {
        private readonly UserMapper _mapper;
        private readonly IUserRepository _repository;

        public UserFacade(UserMapper mapper, IUserRepository repository)
        {
            this._mapper = mapper;
            this._repository = repository;
        }

        public Guid Create(UserDetailModel user)
        {
            var userEntity = _mapper.ToEntity(user);
            _repository.Insert(userEntity);
            return userEntity.Id;
        }

        public Guid CreateOrUpdate(UserDetailModel user)
        {
            return _repository.Exists(user.Id)
                ? Update(user)!.Value                // ! -> can not be null
                : Create(user);
        }

        public void Delete(UserDetailModel user)
        {
            _repository.Remove(user.Id);
        }

        public List<UserListModel> GetAll()
        {
            var entities = _repository.GetAll();
            return _mapper.ToListModel(entities);
        }

        public UserDetailModel? GetById(Guid id)
        {
            var userEntity = _repository.GetById(id);
            if (userEntity == null) return null;
            return _mapper.ToDetailModel((userEntity));
        }

        public Guid? Update(UserDetailModel user)
        {
            var userEntity = _mapper.ToEntity(user);
            return _repository.Update(userEntity);
        }
    }
}
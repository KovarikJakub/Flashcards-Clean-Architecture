using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Storage _storage;

        public UserRepository(Storage storage)
        {
            _storage = storage;
        }

        public bool Exists(Guid id)
        {
            return _storage.Users.Any(user => user.Id == id);
        }

        public UserEntity FirstOrDefault(Expression<Func<UserEntity, bool>> predicate)
        {
            return _storage.Users.AsQueryable().FirstOrDefault(predicate);
        }

        public IList<UserEntity> GetAll()
        {
            return _storage.Users.ToList();
        }

        public UserEntity? GetById(Guid id)
        {
            return _storage.Users.SingleOrDefault(user => user.Id == id);
        }

        public Guid Insert(UserEntity user)
        {
            _storage.Users.Add(user);
            return user.Id;
        }

        public void Remove(Guid id)
        {
            var userToRemove = _storage.Users.Single(user => user.Id == id);
            _storage.Users.Remove(userToRemove);
        }

        public Guid? Update(UserEntity user)
        {
            var existingUser = _storage.Users.Single(userInStorage => userInStorage.Id == user.Id);
            if (existingUser is not null)
            {
                existingUser.Name = user.Name;
                existingUser.Role = user.Role;
                existingUser.PhotographyUrl = user.PhotographyUrl;
                return existingUser.Id;
            }

            return null; // User is not in storage -> can not be updated
        }

        public IEnumerable<UserEntity> Where(Expression<Func<UserEntity, bool>> predicate)
        {
            return _storage.Users.AsQueryable().Where(predicate);
        }
    }
}
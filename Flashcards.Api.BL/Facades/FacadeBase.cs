using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Entities.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.BL.Facades
{
    public class FacadeBase<TRepository, TEntity>(TRepository repository, IUserRepository userRepository)
        where TRepository : IApiRepository<TEntity>
        where TEntity : IEntity
    {
        protected UserEntity GetOwningEntity(string requestorId)
        {
            var requestorUserEntity = userRepository.FirstOrDefault(user => string.Equals(user.IdentityOwnerId, requestorId));
            if (requestorUserEntity is null)
            {
                throw new ArgumentException("This users identity does not exist!");
            }

            return requestorUserEntity;
        }
    }
}

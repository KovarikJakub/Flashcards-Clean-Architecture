using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Entities.Interfaces;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.BL.Facades
{
    public class FacadeBase<TRepository, TEntity>(TRepository repository, IUserRepository userRepository)
        where TRepository : IApiRepository<TEntity>
        where TEntity : IEntity
    {
        protected void ThrowIfUnauthorized(string requestorId)
        {
            if (string.IsNullOrEmpty(requestorId))
            {
                throw new UnauthorizedAccessException("Requestor identity is required.");
            }
        }
    }
}

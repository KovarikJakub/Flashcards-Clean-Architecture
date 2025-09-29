using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public interface IFakeDatabase
    {
        ICardRepository GetCardRepository();

        CardEntity GetCardById(Guid id);
        IList<Guid> CardGuids { get; }
    }
}

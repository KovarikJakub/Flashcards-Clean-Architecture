using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardFacade : IFacade
    {
        List<CardListModel> GetAll();
        CardDetailModel? GetById(Guid id);
        Guid Create(CardDetailModel card);
        Guid CreateOrUpdate(CardDetailModel card);
        Guid? Update(CardDetailModel card);
        void Delete(Guid id);
    }
}

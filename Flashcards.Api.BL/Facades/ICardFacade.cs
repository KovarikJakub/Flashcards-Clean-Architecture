using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardFacade : IFacade
    {
        List<CardListModel> GetAll();
        List<CardListModel> GetByQuery(CardQueryModel query);
        CardDetailModel? GetById(Guid id);
        Guid Create(CardDetailModel card);
        Guid CreateOrUpdate(CardDetailModel card);
        Guid? Update(CardDetailModel card);
        void Delete(Guid id);
    }
}

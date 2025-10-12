using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models.CardCollection;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardCollectionFacade : IFacade
    {
        List<CardCollectionListModel> GetAll();
        CardCollectionDetailModel? GetById(Guid id);
        Guid Create(CardCollectionDetailModel collection);
        Guid CreateOrUpdate(CardCollectionDetailModel collection);
        Guid? Update(CardCollectionDetailModel collection);
        void Delete(Guid id);
    }
}

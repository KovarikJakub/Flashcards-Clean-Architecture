using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardCollectionFacade : IFacade
    {
        List<CardCollectionListModel> GetAll();
        List<CardCollectionListModel> GetByQuery(CardCollectionQueryModel query);
        CardCollectionDetailModel? GetById(Guid id);
        Guid CreateOrUpdate(CardCollectionDetailModel model, IList<string> userRoles, string requestorId);
        Guid Create(CardCollectionCreateModel model, IList<string> userRoles, string requestorId);
        Guid? Update(CardCollectionDetailModel model, IList<string> userRoles, string requestorId);
        void Delete(Guid id, IList<string> userRoles, string? ownerId = null);
    }
}

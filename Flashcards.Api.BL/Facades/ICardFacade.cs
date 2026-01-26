using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardFacade : IFacade
    {
        List<CardListModel> GetAll();
        List<CardListModel> GetByQuery(CardQueryModel query);
        CardDetailModel? GetById(Guid id);
        Guid Create(CardCreateModel card, IList<string> userRoles, string requestorId);
        Guid CreateOrUpdate(CardDetailModel card, IList<string> userRoles, string requestorId);
        Guid? Update(CardDetailModel card, IList<string> userRoles, string requestorId);
        void Delete(Guid id, IList<string> userRoles, string requestorId);
    }
}

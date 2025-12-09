using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardCollectionFacade : IFacade
    {
        List<CardCollectionListModel> GetAll();
        CardCollectionDetailModel? GetById(Guid id);
        Guid CreateOrUpdate(CardCollectionDetailModel recipeModel, IList<string> userRoles, string? ownerId = null);
        Guid Create(CardCollectionDetailModel recipeModel, IList<string> userRoles, string? ownerId = null);
        Guid? Update(CardCollectionDetailModel recipeModel, IList<string> userRoles, string? ownerId = null);
        void Delete(Guid id, IList<string> userRoles, string? ownerId = null);
    }
}

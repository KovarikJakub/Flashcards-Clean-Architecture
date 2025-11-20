using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;

namespace Flashcards.Web.BL.Facades
{
    public class CardCollectionFacade
    {
        private readonly ICardCollectionApiClient apiClient;

        public CardCollectionFacade(ICardCollectionApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<List<CardCollectionListModel>> GetAllAsync()
        {
            var cardCollections = await apiClient.CardCollectionGetAsync();
            return cardCollections.ToList();
        }

    }
}

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

        public async Task<Guid> UpsertAsync(CardCollectionDetailModel model)
        {
            var id = await apiClient.UpsertAsync(model);
            return id;
        }
        
        public async Task DeleteAsync(Guid id)
        {
            await apiClient.CardCollectionDeleteAsync(id);
        }

        public async Task<List<CardCollectionListModel>> GetAllAsync()
        {
            var cardCollections = await apiClient.CardCollectionGetAsync();
            return cardCollections.ToList();
        }
        
        public async Task <CardCollectionDetailModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await apiClient.CardCollectionGetAsync(id);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}

using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;

namespace Flashcards.Web.BL.Facades
{
    public class CardFacade
    {
        private readonly ICardApiClient _apiClient;
        
        public CardFacade(ICardApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        
        public async Task<ICollection<CardListModel>> GetAllAsync()
        {
            var cards = await _apiClient.CardGetAsync();
            return cards.ToList();
        }
        
        public async Task<CardDetailModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var card = await _apiClient.CardGetAsync(id);
                return card;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task DeleteAsync(Guid id)
        {
            await _apiClient.CardDeleteAsync(id);
        }
        
        public async Task<Guid> UpsertAsync(CardDetailModel model)
        {
            return await _apiClient.CardPostAsync(model);
        }
    }    
}

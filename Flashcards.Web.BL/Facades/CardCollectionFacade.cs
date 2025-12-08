using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Models;

namespace Flashcards.Web.BL.Facades
{
    public class CardCollectionFacade
    {
        private readonly ICardCollectionApiClient apiClient;

        public CardCollectionFacade(ICardCollectionApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        /*
        public async Task<List<CardCollectionListModel>> GetAllAsync()
        {
            var cardCollections = await apiClient.CardCollectionGetAsync();
            return cardCollections.ToList();
            
            return Task.FromResult(TestCollections.ToList());
        }
*/
        public async Task DeleteAsync(Guid id)
        {
            await apiClient.CardCollectionDeleteAsync(id);
        }

        // Return a web wrapper so UI can see OwnerId (temporary)
        public async Task<CardCollectionWebModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await apiClient.CardCollectionGetAsync(id);
                // OwnerId left null here — can be set from current user claims in future
                return new CardCollectionWebModel(result, ownerId: null);
            }
            catch
            {
                return null;
            }
        }

        // Backwards-compatible helper for code that expects CardCollectionDetailModel
        public async Task<CardCollectionDetailModel?> GetByIdModelAsync(Guid id)
        {
            var wrapper = await GetByIdAsync(id);
            return wrapper?.ApiModel;
        }

        // Accept the web wrapper, but send only the ApiModel to backend
        
        public async Task<Guid> UpsertAsync(CardCollectionWebModel webModel)
        {
            var id = await apiClient.UpsertAsync(webModel.ApiModel);
            return id;
        }
        
        // Backwards-compatible overload
        public async Task<Guid> UpsertAsync(CardCollectionDetailModel model)
        {
            var id = await apiClient.UpsertAsync(model);
            return id;
        }
        
        // temporary test data method
        public Task<List<CardCollectionListModel>> GetAllAsync()
        {
            return Task.FromResult(TestCollections.ToList());
        }
        
        private static readonly List<CardCollectionListModel> TestCollections = new()
        {
            new CardCollectionListModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"b72b5434-9f35-4e3f-96a8-3fdcbbbc09ee"),
                Title = "title NENAVIDIM IMS",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow
            },
            new CardCollectionListModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"b72b54c8-9f35-4e3f-96a8-3fdcbbbc09ee"),
                Title = "IMS JE SRACKA",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow
            },
            new CardCollectionListModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"b72b54c4-9f35-4e3f-96a8-3fdcbbbc09ee"),
                Title = "PROC EXISTUJE IMS",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow
            },
            new CardCollectionListModel
            {
            Id = Guid.Parse((ReadOnlySpan<char>)"b72b54c4-9f35-4e3f-96a8-3fdcbbbc09ee"),
            Title = "title ahoj",
            AnswerStart = DateTime.UtcNow,
            AnswerEnd = DateTime.UtcNow
        },
        new CardCollectionListModel
        {
            Id = Guid.Parse((ReadOnlySpan<char>)"b72b54c4-9f35-4e3f-96a8-3fdcbbbc09ee"),
            Title = "title cau",
            AnswerStart = DateTime.UtcNow,
            AnswerEnd = DateTime.UtcNow
        }
        };
        
        
    }
}

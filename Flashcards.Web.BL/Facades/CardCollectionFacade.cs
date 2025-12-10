using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Models;

//TODO remove
using Flashcards.Common.Enums;



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
                // Temporary local stub: if the requested id matches our provisional cardCollection, return it directly
                if (cardCollection != null && id == cardCollection.Id)
                {
                    return await Task.FromResult(new CardCollectionWebModel(cardCollection, ownerId: null));
                }

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
            Id = Guid.Parse((ReadOnlySpan<char>)"d4f5c8e2-3c6b-4f1a-9f7e-2b8e5d6c7a9b"),
            Title = "pokus",
            AnswerStart = DateTime.UtcNow,
            AnswerEnd = DateTime.UtcNow,
            
        }
        };
        
        public  CardCollectionDetailModel cardCollection =
            new CardCollectionDetailModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"d4f5c8e2-3c6b-4f1a-9f7e-2b8e5d6c7a9b"),
                Title = "IMS hater",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddHours(1),
                Cards = new List<CardDetailModel>
                {
                    new CardDetailModel
                    {
                        Id = Guid.Parse((ReadOnlySpan<char>)"a1b2c3d4-e5f6-7a8b-9c0d-e1f2a3b4c5d6"),
                        Description = "popisek",
                        AnswerContent = "Praha is capital city of Czechia",
                        QuestionContent = "What is the capital city of Czechia?",
                        AnswerType = ContentType.Text,
                        QuestionType = ContentType.Text,
                        DateCreated = DateTime.UtcNow

                    },
                    new CardDetailModel
                    {
                        Id = Guid.Parse((ReadOnlySpan<char>)"f1e2d3c4-b5a6-7d8e-9f0a-b1c2d3e4f5a6"),
                        Description = "ahoj",
                        AnswerContent = "Ano je docela borec",
                        QuestionContent = "IMS je strasne spatny predmet, ale hruby je docela borec ne?",
                        AnswerType = ContentType.Text,
                        QuestionType = ContentType.Text,
                        DateCreated = DateTime.UtcNow
                    }
                }
            };
        
       
       
    }
}

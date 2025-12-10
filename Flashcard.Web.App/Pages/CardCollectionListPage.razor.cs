using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;


namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionListPage : PageBase
    {

        private CardCollectionDetailModel Model = new CardCollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            AnswerStart = DateTime.Now,
            AnswerEnd = DateTime.Now.AddDays(1),
            Cards = new List<CardDetailModel>()
        };

        private bool showCreateForm = false;
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private ICollection<CardCollectionListModel> CardCollections { get; set; } = new List<CardCollectionListModel>();
        
        protected override async Task OnInitializedAsync()
        {
            
            await LoadData();

            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            CardCollections = await CardCollectionFacade.GetAllAsync();
        }

        private Task OnEditCollection(CardCollectionListModel collection)
        {
            NavigationManager.NavigateTo($"collections/edit/{collection.Id}");
            return Task.CompletedTask;
        }

        private async Task OnDeleteCollection(CardCollectionListModel collection)
        {
            // Example: call facade to delete and reload list
            try
            {
                await CardCollectionFacade.DeleteAsync(collection.Id);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete collection: {ex.Message}");
            }
        }
        
        private void HandlePlayCollection(CardCollectionListModel collection)
        {
            //TODOO correct navigation to play page
            NavigationManager.NavigateTo($"/play/{collection.Id}");
        }
        
        private async Task HandleSubmit(CardCollectionDetailModel model)
        {
            CardCollectionDetailModel newCollection = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                AnswerStart = model.AnswerStart,
                AnswerEnd = model.AnswerEnd,
                Cards = model.Cards
            };
            await CardCollectionFacade.UpsertAsync(newCollection);
        }
        
        private void  HandleCancel()
        {
            showCreateForm = false;
        }

        private void HandleOpenForm()
        {
            showCreateForm = true;
        } 
    }
}

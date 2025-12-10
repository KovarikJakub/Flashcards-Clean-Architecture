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

        private string FilterMy { get; set; } = string.Empty;
        private string FilterAll { get; set; } = string.Empty;

        private List<CardCollectionListModel> FilteredMyCollections =>
            string.IsNullOrWhiteSpace(FilterMy)
                ? CardCollections.ToList()
                : CardCollections.Where(c => c.Title?.Contains(FilterMy.Trim(), StringComparison.OrdinalIgnoreCase) == true).ToList();

        private List<CardCollectionListModel> FilteredAllCollections =>
            string.IsNullOrWhiteSpace(FilterAll)
                ? CardCollections.ToList()
                : CardCollections.Where(c => c.Title?.Contains(FilterAll.Trim(), StringComparison.OrdinalIgnoreCase) == true).ToList();

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
            NavigationManager.NavigateTo($"/play/{collection.Id}");
        }
        
        private async Task HandleSubmit(CardCollectionDetailModel model)
        {
            try 
            {
                CardCollectionDetailModel newCollection = new CardCollectionDetailModel
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    AnswerStart = model.AnswerStart,
                    AnswerEnd = model.AnswerEnd,
                    Cards = model.Cards
                };
                
                showCreateForm = false;
                await CardCollectionFacade.UpsertAsync(newCollection);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create collection: {ex.Message}");
            }
        }
        
        private void  HandleCancel()
        {
            showCreateForm = false;
        }

        private void HandleOpenForm()
        {
            showCreateForm = true;
        } 

        private bool IsCollectionActive(CardCollectionListModel collection)
        {
            var now = DateTime.UtcNow;
            var startUtc = collection.AnswerStart.ToUniversalTime();
            var endUtc = collection.AnswerEnd.ToUniversalTime();
            return now >= startUtc && now <= endUtc;
        }
    }
}

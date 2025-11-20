using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;


namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionListPage
    {
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
    }
}

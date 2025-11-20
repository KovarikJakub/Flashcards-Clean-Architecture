using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;


namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionListPage
    {
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;

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
    }
}

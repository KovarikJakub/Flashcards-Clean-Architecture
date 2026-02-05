using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages
{
    public partial class CardListPage : PageBase
    {
        [Inject] private CardFacade CardFacade { get; set; } = null!;
        [Inject] private UserFacade UserFacade { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

        private List<CardListModel> DisplayedCards { get; set; } = new();

        private int CurrentPage { get; set; } = 1;
        private const int PageSize = 12;
        private bool HasNextPage { get; set; } = false;

        private string SearchString { get; set; } = string.Empty;
        private string ActiveTab { get; set; } = "my";
        
        private ContentType? SelectedTypeFilter { get; set; } = null;
        private string SelectedSort { get; set; } = "default";

        private string? _userId;
        private string? _userName;
        private Guid _internalUserId;

        private bool _showDetailModal = false;
        private CardDetailModel? _selectedCardDetail = null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    _userId = user.FindFirst("sub")?.Value;
                    _userName = user.Identity.Name;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Auth Error: {ex.Message}"); }
            await LoadData();
        }

        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(_userId)) return;

            try
            {
                if (_internalUserId == Guid.Empty)
                {
                    var currentUser = await UserFacade.GetByIdentityIdAsync(_userId);
                    _internalUserId = currentUser.Id;
                }

                Guid? ownerIdFilter = ActiveTab == "my" ? _internalUserId : null;

                string? sortBy = null;
                bool sortAsc = false;

                switch (SelectedSort)
                {
                    case "az": 
                        sortBy = "QuestionContent"; 
                        sortAsc = true; 
                        break;
                    case "za": 
                        sortBy = "QuestionContent"; 
                        sortAsc = false; 
                        break;
                    default: 
                        sortBy = "DateCreated"; 
                        sortAsc = false; 
                        break;
                }

                var query = new CardQueryModel
                {
                    OwnerId = ownerIdFilter,
                    SearchString = SearchString,
                    QuestionType = SelectedTypeFilter,
                    Page = CurrentPage,
                    PageSize = PageSize,
                    SortBy = sortBy,
                    SortAsc = sortAsc
                };

                var result = await CardFacade.GetByQueryAsync(query);
                
                DisplayedCards = result.Items.ToList();
                HasNextPage = DisplayedCards.Count == PageSize;

                if (ActiveTab == "my")
                {
                    foreach (var card in DisplayedCards)
                    {
                        if (string.IsNullOrEmpty(card.OwnerName)) card.OwnerName = _userName ?? "Me";
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"LoadData Error: {ex.Message}"); }
        }

        private async Task GoToNextPage() { if (HasNextPage) { CurrentPage++; await LoadData(); } }
        private async Task GoToPreviousPage() { if (CurrentPage > 1) { CurrentPage--; await LoadData(); } }
        private async Task SetTab(string tab) { if (ActiveTab != tab) { ActiveTab = tab; CurrentPage = 1; await LoadData(); } }
        private async Task OnSearchChanged() { CurrentPage = 1; await LoadData(); }

        private bool IsOwner(CardListModel card)
        {
            if (ActiveTab == "my") return true;
            if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(card.OwnerName)) return false;
            return string.Equals(card.OwnerName, _userName, StringComparison.OrdinalIgnoreCase);
        }

        private async Task OpenDetailModal(CardListModel listModel)
        {
            _selectedCardDetail = null;
            _showDetailModal = true;
            try { _selectedCardDetail = await CardFacade.GetByIdAsync(listModel.Id); } catch { }
        }
        private void CloseDetailModal() { _showDetailModal = false; _selectedCardDetail = null; }

        private Task OnEditCard(CardListModel card) { NavigationManager.NavigateTo($"cards/edit/{card.Id}"); return Task.CompletedTask; }
        private async Task OnDeleteCard(CardListModel card) { await CardFacade.DeleteAsync(card.Id); await LoadData(); }
    }
}
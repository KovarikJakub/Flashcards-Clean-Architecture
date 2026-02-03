using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages
{
    public partial class CardListPage : PageBase
    {
        // --- INJECTIONS ---
        [Inject] private CardFacade CardFacade { get; set; } = null!;
        [Inject] private UserFacade UserFacade { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

        // --- DATA ---
        // Seznam karet zobrazený v Gridu
        private List<CardListModel> DisplayedCards { get; set; } = new();

        // --- PAGING & FILTERS ---
        private int CurrentPage { get; set; } = 1;
        private const int PageSize = 12; // 12 karet na stránku (3 řady po 4)
        private bool HasNextPage { get; set; } = false;

        private string SearchString { get; set; } = string.Empty;
        private string ActiveTab { get; set; } = "my"; // "my" nebo "all"

        // --- USER INFO ---
        private string? _userId;   // Auth0 ID (sub)
        private string? _userName; // Jméno uživatele (pro porovnání vlastnictví)
        private Guid _internalUserId; // Naše DB Guid

        // --- UI STATES ---
        private bool _showCreateForm;
        private bool _showDetailModal = false;
        private CardDetailModel? _selectedCardDetail = null;

        // Model pro vytváření nové karty
        private CardDetailModel _model = new CardDetailModel
        {
            Id = Guid.NewGuid(),
            QuestionType = ContentType.Text,
            AnswerType = ContentType.Text,
            QuestionContent = "",
            AnswerContent = "",
            Description = "",
            DateCreated = DateTime.UtcNow
        };

        // --- LIFECYCLE ---
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
            catch (Exception ex)
            {
                Console.WriteLine($"Auth Error: {ex.Message}");
            }

            await LoadData();
        }

        // --- MAIN DATA LOADING ---
        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(_userId)) return;

            try
            {
                // 1. Získáme interního uživatele (pokud ho ještě nemáme)
                if (_internalUserId == Guid.Empty)
                {
                    var currentUser = await UserFacade.GetByIdentityIdAsync(_userId);
                    _internalUserId = currentUser.Id;
                }

                // 2. Nastavíme filtr vlastníka podle Tabu
                Guid? ownerIdFilter = null;
                if (ActiveTab == "my")
                {
                    ownerIdFilter = _internalUserId;
                }
                else
                {
                    ownerIdFilter = null; // null = Všechny karty (nebo veřejné, záleží na backendu)
                }

                // 3. Sestavíme Query se stránkováním
                var query = new CardQueryModel
                {
                    OwnerId = ownerIdFilter,
                    SearchString = SearchString,
                    Page = CurrentPage,
                    PageSize = PageSize,
                    SortBy = "DateCreated", // Volitelné: řazení
                    SortAsc = false
                };

                // 4. Volání API
                var result = await CardFacade.GetByQueryAsync(query);

                DisplayedCards = result.Items.ToList();

                // 5. Kontrola další stránky (jednoduchá logika)
                // Pokud API vrátilo plný počet karet, pravděpodobně existuje další stránka.
                // Ideálně by PagedResponse mělo mít property TotalPages nebo HasNext.
                HasNextPage = DisplayedCards.Count == PageSize;

                // 6. Kosmetická úprava: Doplnění jména u mých karet, pokud chybí
                if (ActiveTab == "my")
                {
                    foreach (var card in DisplayedCards)
                    {
                        if (string.IsNullOrEmpty(card.OwnerName))
                        {
                            card.OwnerName = _userName ?? "Me";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadData Error: {ex.Message}");
            }
        }

        // --- PAGINATION & TABS HANDLERS ---

        private async Task GoToNextPage()
        {
            if (HasNextPage)
            {
                CurrentPage++;
                await LoadData();
                // Zde by mohl být JS scroll to top
            }
        }

        private async Task GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadData();
            }
        }

        private async Task SetTab(string tab)
        {
            if (ActiveTab != tab)
            {
                ActiveTab = tab;
                CurrentPage = 1; // Reset na první stránku při změně tabu
                await LoadData();
            }
        }

        private async Task OnSearchChanged()
        {
            CurrentPage = 1; // Reset na první stránku při hledání
            await LoadData();
        }

        // --- OWNERSHIP LOGIC ---
        private bool IsOwner(CardListModel card)
        {
            // Pokud jsme v tabu "My Cards", všechno je moje
            if (ActiveTab == "my") return true;

            // V tabu "All Cards" musíme porovnávat jména (protože v modelu není ID)
            if (string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(card.OwnerName))
                return false;

            return string.Equals(card.OwnerName, _userName, StringComparison.OrdinalIgnoreCase);
        }

        // --- MODAL LOGIC ---
        private async Task OpenDetailModal(CardListModel listModel)
        {
            _selectedCardDetail = null; // Reset pro loader
            _showDetailModal = true;
            try
            {
                _selectedCardDetail = await CardFacade.GetByIdAsync(listModel.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modal Error: {ex.Message}");
            }
        }

        private void CloseDetailModal()
        {
            _showDetailModal = false;
            _selectedCardDetail = null;
        }

        // --- CRUD OPERATIONS ---

        private async Task SubmitCreate()
        {
            try
            {
                var newCard = new CardDetailModel
                {
                    Id = Guid.NewGuid(),
                    QuestionType = _model.QuestionType,
                    AnswerType = _model.AnswerType,
                    QuestionContent = _model.QuestionContent,
                    AnswerContent = _model.AnswerContent,
                    Description = _model.Description,
                    DateCreated = DateTime.UtcNow,
                    // Owner se nastavuje na backendu
                };

                await CardFacade.UpsertAsync(newCard);

                // Reset formuláře
                _showCreateForm = false;
                _model = new CardDetailModel
                {
                    Id = Guid.NewGuid(),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "",
                    AnswerContent = "",
                    Description = "",
                    DateCreated = DateTime.UtcNow
                };

                // Po vytvoření obnovíme data (zůstaneme na aktuální stránce nebo jdeme na první?)
                // Obvykle chceme vidět novou kartu -> reset na stranu 1
                if (ActiveTab == "my")
                {
                    CurrentPage = 1;
                    await LoadData();
                }
                else
                {
                    // Pokud jsem vytvořil kartu v "All", přepnu se na "My" abych ji viděl
                    await SetTab("my");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create Error: {ex.Message}");
            }
        }

        private async Task OnDeleteCard(CardListModel card)
        {
            try
            {
                await CardFacade.DeleteAsync(card.Id);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
            }
        }

        private Task OnEditCard(CardListModel card)
        {
            NavigationManager.NavigateTo($"cards/edit/{card.Id}");
            return Task.CompletedTask;
        }

        private void HandleCancel() => _showCreateForm = false;
        private void HandleOpenForm() => _showCreateForm = true;
    }
}
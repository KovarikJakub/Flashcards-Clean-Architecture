using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages
{
    public partial class CardListPage : PageBase
    {
        [Inject]
        private CardFacade CardFacade { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private IJSRuntime Js { get; set; } = null!;
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

        private ICollection<CardListModel> CardsAll { get; set; } = new List<CardListModel>();
        private ICollection<CardListModel> CardsMine { get; set; } = new List<CardListModel>();
        
        private string FilterMy { get; set; } = string.Empty;
        private string FilterAll { get; set; } = string.Empty;

        private bool _showCreateForm = false;
        private ElementReference _scrollContainerMy;
        private ElementReference _scrollContainerAll;
        private IJSObjectReference? _module;
        private string? _userId;

        private CardDetailModel _model = new CardDetailModel
        {
            Id = Guid.NewGuid(),
            QuestionType = ContentType.Text,
            AnswerType = ContentType.Text,
            QuestionContent = string.Empty,
            AnswerContent = string.Empty,
            DateCreated = DateTime.UtcNow,
            Description = null
        };

        // Filtrování
        private List<CardListModel> FilteredMyCards =>
            string.IsNullOrWhiteSpace(FilterMy)
                ? CardsMine.ToList()
                : CardsMine.Where(c => 
                    (c.QuestionContent?.Contains(FilterMy.Trim(), StringComparison.OrdinalIgnoreCase) == true) || 
                    (c.Description?.Contains(FilterMy.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                  ).ToList();

        private List<CardListModel> FilteredAllCards =>
            string.IsNullOrWhiteSpace(FilterAll)
                ? CardsAll.ToList()
                : CardsAll.Where(c => 
                    (c.QuestionContent?.Contains(FilterAll.Trim(), StringComparison.OrdinalIgnoreCase) == true) || 
                    (c.Description?.Contains(FilterAll.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                  ).ToList();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                     _userId = user.FindFirst("sub")?.Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve auth state: {ex.Message}");
            }

            await LoadData();
            await base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            // Načteme všechny karty
            CardsAll = await CardFacade.GetAllAsync();

            // TODO: Až bude ve fasádě metoda GetByOwnerAsync, odkomentovat toto:
            // CardsMine = await CardFacade.GetByOwnerAsync(_userId);
            
            // PROZATÍM: Necháme seznam prázdný, aby kód nepadal
            CardsMine = new List<CardListModel>();
        }

        private Task OnEditCard(CardListModel card)
        {
            NavigationManager.NavigateTo($"cards/edit/{card.Id}");
            return Task.CompletedTask;
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
                Console.WriteLine($"Failed to delete card: {ex.Message}");
            }
        }

        private async Task HandleSubmit(CardDetailModel model)
        {
            try 
            {
                CardDetailModel newCard = new CardDetailModel
                {
                    Id = Guid.NewGuid(),
                    QuestionType = model.QuestionType,
                    AnswerType = model.AnswerType,
                    QuestionContent = model.QuestionContent,
                    AnswerContent = model.AnswerContent,
                    DateCreated = DateTime.UtcNow,
                    Description = model.Description
                };
                
               _showCreateForm = false;
                await CardFacade.UpsertAsync(newCard);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create card: {ex.Message}");
            }
        }
        
        private void HandleCancel()
        {
            _showCreateForm = false;
        }

        private void HandleOpenForm()
        {
            _showCreateForm = true;
        } 

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await Js.InvokeAsync<IJSObjectReference>(
                        "import", "/js/Scroll.js");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load JS module: {ex.Message}");
                }
            }
        }

        private async Task ScrollLeft(ElementReference container)
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("scrollLeft", container, 450);
            }
        }

        private async Task ScrollRight(ElementReference container)
        {
            if (_module is not null)
            {
                await _module.InvokeVoidAsync("scrollRight", container, 450);
            }
        }
    }
}
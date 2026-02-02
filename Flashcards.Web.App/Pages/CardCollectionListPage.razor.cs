using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionListPage : PageBase
    {
        // Injections
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private IJSRuntime Js { get; set; } = null!; 
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
        
        // variables and properties
        private bool _showCreateForm;
        private ElementReference _scrollContainerMy;
        private ElementReference _scrollContainerAll;
        private IJSObjectReference? _module;
        private string? _userId;
        private ICollection<CardCollectionListModel> CardCollectionsAll { get; set; } = new List<CardCollectionListModel>();
        private ICollection<CardCollectionListModel> CardCollectionsMine { get; set; } = new List<CardCollectionListModel>();
        private string _filterMy = string.Empty;
        private string FilterMy
        {
            get => _filterMy;
            set
            {
                _filterMy = value;
                _ = OnFilterChanged();
            }
        }
        private string _filterAll = string.Empty;

        private string FilterAll
        {
            get => _filterAll;
            set
            {
                _filterAll = value;
                _ = OnFilterChanged();
            }
        } 
        
        private CardCollectionDetailModel _model = new CardCollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            AnswerStart = DateTime.Now,
            AnswerEnd = DateTime.Now.AddDays(1),
            Cards = new List<CardDetailModel>()
        };
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // get current user identity
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
            if (string.IsNullOrEmpty(_userId))
            {
                // No user logged in, show empty collection
                CardCollectionsMine = Array.Empty<CardCollectionListModel>();
                return;
            }

            // get currently signed user
            var currentUser = await UserFacade.GetByIdentityIdAsync(_userId);
            
            // create query to obtain his card collections
            var queryJustMine = new CardCollectionQueryModel
            {
                OwnerId = currentUser.Id,
                SearchString = FilterMy                             // by card collection title
            };
            var cardCollectionsOfCurrentUser = await CardCollectionFacade.GetByQueryAsync(queryJustMine);
            CardCollectionsMine = cardCollectionsOfCurrentUser.Items;
            
            // create query to obtain all other card collections
            var queryExcludeMine = new CardCollectionQueryModel
            {
                ExcludeOwnerId = currentUser.Id,             
                SearchString = FilterAll                                // by card collection title
            };
            var cardCollectionsExcludingCurrentUser = await CardCollectionFacade.GetByQueryAsync(queryExcludeMine);
            CardCollectionsAll = cardCollectionsExcludingCurrentUser.Items;
        }
        
        // navigation handlers
        private Task OnEditCollection(CardCollectionListModel collection)
        {
            NavigationManager.NavigateTo($"collections/edit/{collection.Id}");
            return Task.CompletedTask;
        }
        
        private void HandlePlayCollection(CardCollectionListModel collection)
        {
            NavigationManager.NavigateTo($"/play/{collection.Id}");
        }

        // action handlers
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
                
               _showCreateForm = false;
                await CardCollectionFacade.UpsertAsync(newCollection);
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create collection: {ex.Message}");
            }
        }
        
        private async Task OnFilterChanged()
        {
            await LoadData();
            StateHasChanged();
        }
        
        private void  HandleCancel()
        {
            _showCreateForm = false;
        }

        private void HandleOpenForm()
        {
            _showCreateForm = true;
        } 

        // checks if current date is within answer period
        private static bool IsCollectionActive(CardCollectionListModel collection)            
        {
            var now = DateTime.Now;
            var startUtc = collection.AnswerStart;
            var endUtc = collection.AnswerEnd;
            return now >= startUtc && now <= endUtc;
        }
        
        // JS for scrolling
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _module = await Js.InvokeAsync<IJSObjectReference>("import", "/js/Scroll.js");
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

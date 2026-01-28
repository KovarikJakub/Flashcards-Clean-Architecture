using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;

namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionListPage : PageBase
    {
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
        
        private ICollection<CardCollectionListModel> CardCollectionsAll { get; set; } = new List<CardCollectionListModel>();
        private ICollection<CardCollectionListModel> CardCollectionsMine { get; set; } = new List<CardCollectionListModel>();
        private string FilterMy { get; set; } = string.Empty;
        private string FilterAll { get; set; } = string.Empty;
        
        private bool _showCreateForm = false;
        private ElementReference _scrollContainerMy;
        private ElementReference _scrollContainerAll;
        private IJSObjectReference? _module;
        private string? _userId;

        private CardCollectionDetailModel _model = new CardCollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            AnswerStart = DateTime.Now,
            AnswerEnd = DateTime.Now.AddDays(1),
            Cards = new List<CardDetailModel>()
        };

        private List<CardCollectionListModel> FilteredMyCollections =>
            string.IsNullOrWhiteSpace(FilterMy)
                ? CardCollectionsMine.ToList()
                : CardCollectionsMine.Where(c => c.Title?.Contains(FilterMy.Trim(), StringComparison.OrdinalIgnoreCase) == true).ToList();

        private List<CardCollectionListModel> FilteredAllCollections =>
            string.IsNullOrWhiteSpace(FilterAll)
                ? CardCollectionsAll.ToList()
                : CardCollectionsAll.Where(c => c.Title?.Contains(FilterAll.Trim(), StringComparison.OrdinalIgnoreCase) == true).ToList();

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
            CardCollectionsAll = await CardCollectionFacade.GetAllAsync();

            if (string.IsNullOrEmpty(_userId))
            {
                // No user logged in, show empty collection
                CardCollectionsMine = Array.Empty<CardCollectionListModel>();
                return;
            }

            // get currently signed user:
            var currentUser = await UserFacade.GetByIdentityIdAsync(_userId);
            // return his cards:
            var queryJustMine = new CardCollectionQueryModel
            {
                OwnerId = currentUser.Id
            };
            // TODO: QUERY endpoints now return PagedResult<TEntity> and not IList<TEntity> !!!
            //CardCollectionsMine = await CardCollectionFacade.GetByQueryAsync(queryJustMine);
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
                
               _showCreateForm = false;
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
            _showCreateForm = false;
        }

        private void HandleOpenForm()
        {
            _showCreateForm = true;
        } 

        private bool IsCollectionActive(CardCollectionListModel collection)
        {
            var now = DateTime.Now;
            var startUtc = collection.AnswerStart;
            var endUtc = collection.AnswerEnd;
            return now >= startUtc && now <= endUtc;
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    Console.WriteLine("Attempting to load JS module...");
                    _module = await Js.InvokeAsync<IJSObjectReference>(
                        "import", "/js/Scroll.js");
                    Console.WriteLine("JS module loaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load JS module: {ex.Message}");
                }
            }
        }

        private async Task ScrollLeft(ElementReference container)
        {
            Console.WriteLine("ScrollLeft called.");
            if (_module is not null)
            {
                Console.WriteLine("Invoking scrollLeft JS function...");
                await _module.InvokeVoidAsync("scrollLeft", container, 450);
            }
            else
            {
                Console.WriteLine("_module is null. JS function not invoked.");
            }
        }

        private async Task ScrollRight(ElementReference container)
        {
            Console.WriteLine("ScrollRight called.");
            if (_module is not null)
            {
                Console.WriteLine("Invoking scrollRight JS function...");
                await _module.InvokeVoidAsync("scrollRight", container, 450);
            }
            else
            {
                Console.WriteLine("_module is null. JS function not invoked.");
            }
        }
    }
}

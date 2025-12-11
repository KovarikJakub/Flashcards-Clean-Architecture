using Flashcards.Common.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages
{
    public partial class Play : PageBase
    {
        [Parameter]
        public Guid Id { get; init; }
        public string? ownerId { get; set; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        
        private static CardCollectionDetailModel? _cardCollection { get; set; }
        
        [Inject]
        private LessonFacade LessonFacade { get; set; } = null!;
        
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        
        [Inject]
        private UserFacade UserFacade { get; set; } = null!;
        
        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

        private LessonDetailModel? newLesson;
        private UserDetailModel? _currentUser;
        private bool _showStats = false;
        private string? _userId;
        protected override async Task OnInitializedAsync()
        {
            _cardCollection = await CardCollectionFacade.GetByIdAsync(Id);
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    _userId = user.FindFirst("sub")?.Value;
                    _currentUser = await UserFacade.GetByOwnerIdAsync(ownerId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve auth state: {ex.Message}");
            }
            newLesson = new LessonDetailModel
                {
                    Id = Guid.NewGuid(),
                    User = _currentUser,
                    CardCollection = _cardCollection,
                    Start = DateTime.Now,
                    Correct = 0,
                    Seen = 0,
                    Wrong = 0
                };
        }

        private int _index = 0;
        private bool _isFlipped = false;

        private enum AnswerState { None, Correct, Wrong }
        private readonly Dictionary<Guid, AnswerState> _answers = new();

        private bool IsAnswered(Guid cardId) => _answers.TryGetValue(cardId, out var s) && s != AnswerState.None;
        private AnswerState GetAnswerState(Guid cardId) => _answers.TryGetValue(cardId, out var s) ? s : AnswerState.None;

        private Task HandleMarkCorrectAsync(Guid cardId)
        {
            HandleClickNext();
            return HandleAnswerAsync(cardId, AnswerState.Correct);
        }

        private Task HandleMarkWrongAsync(Guid cardId)
        {
            HandleClickNext();
            return HandleAnswerAsync(cardId, AnswerState.Wrong);
        }

        private Task HandleAnswerAsync(Guid cardId, AnswerState state)
        {
            if (IsAnswered(cardId)) return Task.CompletedTask;

            _answers[cardId] = state;

            if (newLesson != null)
            {
                newLesson.Seen++;
                if (state == AnswerState.Correct) newLesson.Correct++;
                else if (state == AnswerState.Wrong) newLesson.Wrong++;
            }

            InvokeAsync(StateHasChanged);
            
            return Task.CompletedTask;
        }

        private void HandleClickBack()
        {
            if (_index > 0) _index--;
        }

        private async Task HandleClickNext()
        {
            if (_cardCollection != null && _index + 1 < _cardCollection.Cards.Count) _index++;
            else
            {
                try
                {
                    await LessonFacade.UpsertAsync(newLesson);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                _showStats = true;
            }
        }
        
        private void FlipCard()
        {
           _isFlipped = !_isFlipped;
        }
        
        private void HandlePlayAgain()
        {
            newLesson.Correct = 0;
            newLesson.Wrong = 0;
            newLesson.Seen = 0;
            _index = 0;
            _isFlipped = false;
            _showStats = false;
            _answers.Clear();
            NavigationManager.NavigateTo($"/play/{Id}");
        }
        
        private void HandleTryDifferentCollection()
        {
            NavigationManager.NavigateTo("/collections");
        }
    }
}

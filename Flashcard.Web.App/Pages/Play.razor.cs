using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages
{
    public partial class Play : PageBase
    {
        [Parameter]
        public Guid Id { get; init; }
        
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        
        private static CardCollectionDetailModel? _cardCollection { get; set; }
        
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;

        private LessonDetailModel? newLesson;
        private bool _showStats = false;

        protected override async Task OnInitializedAsync()
        {
            _cardCollection = await CardCollectionFacade.GetByIdAsync(Id);        
            newLesson = new LessonDetailModel
                {
                    Id = Guid.NewGuid(),
                    User = CurrentUser,
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

        private void HandleClickNext()
        {
            if (_cardCollection != null && _index + 1 < _cardCollection.Cards.Count) _index++;
            else _showStats = true;
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
           
        // TODO remove dummy user
         static UserDetailModel CurrentUser =
            new UserDetailModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"123e4567-e89b-12d3-a456-426614174000"),
                Name = "pepa",
                PhotographyUrl = null,
                Role = RoleType.User
            };
    }
}

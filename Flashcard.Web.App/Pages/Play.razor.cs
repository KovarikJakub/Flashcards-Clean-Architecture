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
        
        public static CardCollectionDetailModel? _cardCollection { get; set; }
        
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            _cardCollection = await CardCollectionFacade.GetByIdModelAsync(Id);        // Zmenit na getbyId, nepouzivat  provizorni webmodel
        }

        private int _index = 0;
        private bool _isFlipped = false;
        
        private void HandleClickCorrect()
        {
            CurrentLesson.Correct++;
        }
        
        private void HandleClickWrong()
        {
            CurrentLesson.Wrong++;
        }

        private void HandleClickBack()
        {
            if (_index - 1 >= 0) _index--;
        } 
        
        private void HandleClickNext()
        {
            if (_index + 1 < _cardCollection.Cards.Count) _index++;
        }
        
        private void FlipCard()
        {
           _isFlipped = !_isFlipped;
        }
        
        // Dummy data
        // oddelat static, a getbyId()
        /*
        private static CardCollectionDetailModel _cardCollection =
            new CardCollectionDetailModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"d4f5c8e2-3c6b-4f1a-9f7e-2b8e5d6c7a9b"),
                Title = "IMS hater",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddHours(1),
                Cards = new List<CardDetailModel>
                {
                    new CardDetailModel
                    {
                        Id = Guid.Parse((ReadOnlySpan<char>)"a1b2c3d4-e5f6-7a8b-9c0d-e1f2a3b4c5d6"),
                        Description = "popisek",
                        AnswerContent = "Praha is capital city of Czechia",
                        QuestionContent = "What is the capital city of Czechia?",
                        AnswerType = ContentType.Text,
                        QuestionType = ContentType.Text,
                        DateCreated = DateTime.UtcNow

                    },
                    new CardDetailModel
                    {
                        Id = Guid.Parse((ReadOnlySpan<char>)"f1e2d3c4-b5a6-7d8e-9f0a-b1c2d3e4f5a6"),
                        Description = "ahoj",
                        AnswerContent = "Ano je docela borec",
                        QuestionContent = "IMS je strasne spatny predmet, ale hruby je docela borec ne?",
                        AnswerType = ContentType.Text,
                        QuestionType = ContentType.Text,
                        DateCreated = DateTime.UtcNow
                    }
                }
            };
        */
            
         static UserDetailModel CurrentUser =
            new UserDetailModel
            {
                Id = Guid.Parse((ReadOnlySpan<char>)"123e4567-e89b-12d3-a456-426614174000"),
                Name = "pepa",
                PhotographyUrl = null,
                Role = RoleType.User
            };
        
        public LessonDetailModel CurrentLesson =
            new LessonDetailModel
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
}


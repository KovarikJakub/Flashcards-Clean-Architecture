using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Flashcards.Web.BL.Models;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionEditPage : PageBase
    {
        [Parameter]
        public Guid? Id { get; set; }

        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        public CardCollectionDetailModel Model { get; set; } = new CardCollectionDetailModel
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            AnswerStart = DateTime.Now,
            AnswerEnd = DateTime.Now.AddHours(1)
        };

        public List<CardDetailModel> Cards { get; set; } = new List<CardDetailModel>();

        public bool IsCreate => Id == null;

        protected override async Task OnInitializedAsync()
        {
            if (!IsCreate && Id.HasValue)
            {
                var wrapper = await CardCollectionFacade.GetByIdAsync(Id.Value);
                if (wrapper != null)
                {
                    Model = wrapper.ApiModel;
                    Cards = Model.Cards.ToList();
                }
            }
            else
            {
                // create mode: start with one empty card
                Cards.Add(new CardDetailModel
                {
                    Id = Guid.NewGuid(),
                    QuestionType = Flashcards.Common.Enums.ContentType.Text,
                    AnswerType = Flashcards.Common.Enums.ContentType.Text,
                    QuestionContent = string.Empty,
                    AnswerContent = string.Empty,
                    DateCreated = DateTime.Now
                });
                Model.Cards = Cards;
            }

            await base.OnInitializedAsync();
        }

        public void AddCard()
        {
            var card = new CardDetailModel
            {
                Id = Guid.NewGuid(),
                QuestionType = Flashcards.Common.Enums.ContentType.Text,
                AnswerType = Flashcards.Common.Enums.ContentType.Text,
                QuestionContent = string.Empty,
                AnswerContent = string.Empty,
                DateCreated = DateTime.Now
            };
            Cards.Add(card);
        }

        public void RemoveCard(CardDetailModel card)
        {
            Cards.Remove(card);
        }

        public DateTime AnswerStartLocal
        {
            get => Model.AnswerStart;
            set => Model.AnswerStart = value;
        }

        public DateTime AnswerEndLocal
        {
            get => Model.AnswerEnd;
            set => Model.AnswerEnd = value;
        }

        public async Task Save()
        {
            Model.Cards = Cards;
            // wrap the model with possible OwnerId (null for now)
            var webModel = new CardCollectionWebModel(Model, ownerId: null);
            await CardCollectionFacade.UpsertAsync(webModel);
            NavigationManager.NavigateTo($"/collections");
        }
    }
}

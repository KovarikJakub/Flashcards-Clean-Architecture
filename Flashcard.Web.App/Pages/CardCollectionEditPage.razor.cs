using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages
{
    public partial class CardCollectionEditPage : PageBase
    {
        [Parameter]
        public Guid Id { get; set; }
        
        [Inject]
        private CardCollectionFacade CardCollectionFacade { get; set; } = null!;
        
        [Inject]
        private CardFacade CardFacade { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        private CardCollectionDetailModel _cardCollection { get; set; }

         protected override async Task OnInitializedAsync()
         {
                await LoadData();
                await base.OnInitializedAsync();
        }
         
         private async Task LoadData()
         {
             _cardCollection = await CardCollectionFacade.GetByIdAsync(Id);
         }
         
         private async Task SaveCollection()
         {
             try
             {
                 await CardCollectionFacade.UpsertAsync(_cardCollection);
                 NavigationManager.NavigateTo("/collections");
             }
             catch (Exception exception)
             {
                 Console.WriteLine(exception);
                 throw;
             }
         }
         
         private async Task DeleteCard(CardDetailModel card)
         {
             _cardCollection.Cards.Remove(card);
             await CardCollectionFacade.UpsertAsync(_cardCollection);
             await LoadData();
         }
        
        private CardDetailModel NewCard = new()
        {
            Id = Guid.NewGuid(),
            QuestionType = ContentType.Text, 
            AnswerType = ContentType.Text,
            QuestionContent = "",
            AnswerContent = "",
            DateCreated = DateTime.UtcNow,
            Description = ""
        };
        
        private async Task HandleAddNewCard(CardDetailModel card)
        {
            try
            {
                _cardCollection.Cards.Add(NewCard);
                await CardCollectionFacade.UpsertAsync(_cardCollection);
                NewCard = new CardDetailModel
                {
                    Id = Guid.NewGuid(),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "",
                    AnswerContent = "",
                    DateCreated = DateTime.UtcNow,
                    Description = ""
                };
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            finally
            {
                await LoadData();
            }
        }
    }
}

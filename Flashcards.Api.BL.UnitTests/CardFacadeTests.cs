using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Repositories;
using NSubstitute;

namespace Flashcards.Api.BL.UnitTests
{
    public class CardFacadeTests
    {
        [Fact]
        public void Delete_Calls_Method_On_Repository_Correctly()
        {
            // Arrange
            var fakeRepository = Substitute.For<ICardRepository>();

            var mapper = new CardMapper();
            var facade = new CardFacade(fakeRepository, mapper);
            var cardGuid = Guid.NewGuid();
            
            // Act
            facade.Delete(cardGuid);

            // Assert
            fakeRepository.Received().Remove(cardGuid);
        }
    }
}

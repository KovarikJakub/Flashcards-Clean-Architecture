using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class UserRepositoryTests
    {
        private readonly IFakeDatabase fakeDb;

        public  UserRepositoryTests()
        {
            fakeDb = new InMemoryFakeDatabase();
        }

        public bool AreUserEqual(UserEntity user1, UserEntity user2)
        {
            return user1.Id == user2.Id &&
                   user1.Name == user2.Name &&
                   user1.PhotographyUrl == user2.PhotographyUrl &&
                   user1.Role == user2.Role;
        }
        
        [Fact]
        public void GetById_Existing_User()
        {
            // Arrange
            var repo = fakeDb.GetUserRepository();
            
            // Act
            var requestedId = fakeDb.UserGuids[0];
            var user = repo.GetById(requestedId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(user.Id, requestedId);
            
            var expectedUser = fakeDb.GetUserById(requestedId);
            Assert.True(AreUserEqual(user, expectedUser));
        }

        [Fact]
        public void GetById_Non_Existing_User()
        {
            // Arrange
            var repo = fakeDb.GetUserRepository();
            var nonExistingId = Guid.NewGuid();
            
            // Act
            var user = repo.GetById(nonExistingId);
            
            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void Delete_Existing_User()
        {
            // Arrange
            var repo = fakeDb.GetUserRepository();
            var idToRemove = fakeDb.UserGuids[0];
            
            // Act
            repo.Remove(idToRemove);
            
            // Assert
            Assert.Null(fakeDb.GetUserById(idToRemove));
        }

        [Fact]
        public void Insert_New_User()
        {
            // Arrange
            var repo = fakeDb.GetUserRepository();
            var new_user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = "Kuba",
                PhotographyUrl = null,
                Role = RoleType.User

            };

            // Act
            var new_id = repo.Insert(new_user);

            // Assert
            Assert.NotNull(fakeDb.GetUserById(new_id));
        }

        [Fact]
        public void Update_Existing_User()
        {
            // Arrange
            var repo = fakeDb.GetUserRepository();

            var userId = fakeDb.UserGuids[0];
            var user = repo.GetById(userId);
            user.Name = "Ondra";
            
            // Act
            repo.Update(user);
            
            // Assert
            var expectedUser = fakeDb.GetUserById(userId);
            Assert.Equal(user.Name, expectedUser.Name);
        }
    }
}
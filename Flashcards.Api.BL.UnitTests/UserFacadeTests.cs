using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models.User;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Flashcards.Api.BL.UnitTests
{
    public class UserFacadeTests
    {
        [Fact]
        public void Delete_Calls_Correct_Method_On_Repository()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);
            var userId = Guid.NewGuid();
            
            // act
            facade.Delete(userId);
            
            // assert
            fakeRepo.Received(1).Remove(userId);       
        }
        
        [Fact]
        public void Create_New_User()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);
            var newUser = new UserDetailModel()
            {
                Id = Guid.NewGuid(),
                Name = "Petr",
                PhotographyUrl = null,
                Role = RoleType.User
            };

            // Act
            var id = facade.Create(newUser);
            
            // Assert
            fakeRepo.Received(1).Insert(Arg.Is<UserEntity>(user =>
                user.Id == newUser.Id &&
                user.Name == newUser.Name &&
                user.PhotographyUrl == newUser.PhotographyUrl &&
                user.Role == newUser.Role));

            Assert.Equal(id, newUser.Id);
        }

        [Fact]
        public void CreateOrUpdate_On_New_User()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);
            var newUser = new UserDetailModel()
            {
                Id = Guid.NewGuid(),
                Name = "Petr",
                PhotographyUrl = null,
                Role = RoleType.User
            };

            // Act
            var id = facade.CreateOrUpdate(newUser);
            
            // Assert
            fakeRepo.Received(1).Insert(Arg.Is<UserEntity>(user =>
                user.Id == newUser.Id &&
                user.Name == newUser.Name &&
                user.PhotographyUrl == newUser.PhotographyUrl &&
                user.Role == newUser.Role));

            Assert.Equal(id, newUser.Id);
        }
        
        [Fact]
        public void CreateOrUpdate_On_Existing_User()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            fakeRepo.Exists(Arg.Any<Guid>()).Returns(true);
            fakeRepo.Update(Arg.Any<UserEntity>()).Returns(Guid.NewGuid());

            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);

            var existingUser = new UserDetailModel()
            {
                Id = Guid.NewGuid(),
                Name = "Petr",
                PhotographyUrl = null,
                Role = RoleType.User
            };

            existingUser.Name = "Igor";

            // Act
            facade.CreateOrUpdate(existingUser);
    
            // Assert
            fakeRepo.Received(1).Update(Arg.Is<UserEntity>(user =>
                user.Id == existingUser.Id &&
                user.Name == "Igor"));
        }

        [Fact]
        public void GetById_Non_Existing_User()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            fakeRepo.GetById(Arg.Any<Guid>()).Returns((UserEntity?)null);
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);
            var id = Guid.NewGuid();
            
            // Act
            var result = facade.GetById(id);

            // Assert
            fakeRepo.Received(1).GetById(id);
            Assert.Null(result);
        }
        
        [Fact]
        public void GetById_On_Existing_User()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            var existingUser = new UserEntity()
            {
                Name = "David",
                Id = Guid.NewGuid(),
                PhotographyUrl = null,
                Role = RoleType.User
            };
            fakeRepo.GetById(Arg.Any<Guid>()).Returns(existingUser);
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);
            
            // Act
            var result = facade.GetById(existingUser.Id);

            // Assert
            fakeRepo.Received(1).GetById(existingUser.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetAll_On_Existing_Users()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);
            var user1 = new UserEntity()
            {
                Name = "David",
                Id = Guid.NewGuid(),
                PhotographyUrl = null,
                Role = RoleType.User
            };
            var user2 = new UserEntity()
            {
                Name = "Jakub",
                Id = Guid.NewGuid(),
                PhotographyUrl = null,
                Role = RoleType.User
            };
            List<UserEntity> list = new List<UserEntity>();
            list.Add(user1);
            list.Add(user2);
            fakeRepo.GetAll().Returns(list);
            
            // Act
            var result = facade.GetAll();

            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.Equal(2, list.Count);
            Assert.Contains(result, user => user.Id == user1.Id && user.Name == "David");
            Assert.Contains(result, user => user.Id == user2.Id && user.Name == "Jakub");
        }
        
        [Fact]
        public void GetAll_On_Empty_Repo()
        {
            // Arrange
            var fakeRepo = Substitute.For<IUserRepository>();
            fakeRepo.GetAll().Returns(new List<UserEntity>()); 
            var mapper = new UserMapper();
            var facade = new UserFacade(mapper, fakeRepo);

            // Act
            var result = facade.GetAll();

            // Assert
            Assert.NotNull(result);       
            Assert.Empty(result);       
            fakeRepo.Received(1).GetAll();  
        }
    }
}


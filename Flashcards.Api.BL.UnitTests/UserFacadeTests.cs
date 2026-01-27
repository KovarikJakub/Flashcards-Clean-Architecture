using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.BL.Services;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using NSubstitute;

namespace Flashcards.Api.BL.UnitTests
{
    public class UserFacadeTests
    {
        private readonly IUserRepository _fakeRepo;
        private readonly IIdentityService _fakeIdentityService;
        private readonly UserMapper _mapper;
        private readonly UserFacade _facade;

        public UserFacadeTests()
        {
            _fakeRepo = Substitute.For<IUserRepository>();
            _fakeIdentityService = Substitute.For<IIdentityService>();

            _mapper = new UserMapper();
            _facade = new UserFacade(_fakeRepo, _fakeIdentityService, _mapper);
        }

        private UserCreateModel CreateUserCreateModel()
        {
            var newUser = new UserCreateModel()
            {
                Name = "Petr",
                PhotoUrl = null,
                Role = RoleType.User,
                IdentityName = "PetrId",
                Password = "Petr123"
            };

            return newUser;
        }

        private UserDetailModel CreateUserDetailModel(Guid id)
        {
            var newUser = new UserDetailModel()
            {
                Id = id,
                Name = "Petr",
                PhotoUrl = null,
                Role = RoleType.User
            };

            return newUser;
        }


        [Fact]
        public void Delete_Calls_Correct_Method_On_Repository()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var user = new UserEntity()
            {
                Id = userId,
                IdentityOwnerId = "PetrId",
                Name = "Petr",
                Role = RoleType.User
            };
            _fakeRepo.GetById(userId).Returns(user);

            // act
            _facade.Delete(userId, new List<string>(), "abc");
            
            // assert
            _fakeIdentityService.Received(1).DeleteIdentityAsync("PetrId");
            _fakeRepo.Received(1).Remove(Arg.Any<Guid>());       
        }
        
        [Fact]
        public void Create_New_User()
        {
            // Arrange
            var newUser = CreateUserCreateModel();
            _fakeIdentityService.CreateIdentityAsync(Arg.Any<string>(), Arg.Any<string>(), "User")
                .Returns("PetrId");
            // Act
            var id = _facade.Create(newUser, new List<string>(), "abc");
            
            // Assert
            _fakeRepo.Received(1).Insert(Arg.Is<UserEntity>(user =>
                user.Id == id &&
                user.Name == newUser.Name &&
                user.PhotoUrl == newUser.PhotoUrl &&
                user.IdentityOwnerId == newUser.IdentityName &&
                user.Role == newUser.Role));
            _fakeIdentityService.Received(1).CreateIdentityAsync(newUser.IdentityName, newUser.Password, "User");
        }

        [Fact]
        public void GetById_Non_Existing_User()
        {
            // Arrange
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns((UserEntity?)null);
            var id = Guid.NewGuid();
            
            // Act
            var result = _facade.GetById(id);

            // Assert
            _fakeRepo.Received(1).GetById(id);
            Assert.Null(result);
        }
        
        [Fact]
        public void GetById_On_Existing_User()
        {
            // Arrange
            var existingUser = CreateUserDetailModel(Guid.NewGuid());
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns(_mapper.ToEntity(existingUser, "abc"));
            
            // Act
            var result = _facade.GetById(existingUser.Id);

            // Assert
            _fakeRepo.Received(1).GetById(existingUser.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetAll_On_Existing_Users()
        {
            // Arrange
            var user1 = CreateUserDetailModel(Guid.NewGuid());
            var user2 = CreateUserDetailModel(Guid.NewGuid());
            List<UserEntity> list = new List<UserEntity>();
            list.Add(_mapper.ToEntity(user1, "abc"));
            list.Add(_mapper.ToEntity(user2, "abc"));
            _fakeRepo.GetAll().Returns(list);
            
            // Act
            var result = _facade.GetAll();

            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.Equal(2, list.Count);
            Assert.Contains(result, user => user.Id == user1.Id);
            Assert.Contains(result, user => user.Id == user2.Id);
        }
        
        [Fact]
        public void GetAll_On_Empty_Repo()
        {
            // Arrange
            _fakeRepo.GetAll().Returns(new List<UserEntity>()); 

            // Act
            var result = _facade.GetAll();

            // Assert
            Assert.NotNull(result);       
            Assert.Empty(result);       
            _fakeRepo.Received(1).GetAll();  
        }
    }
}


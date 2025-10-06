using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models.User;


namespace Flashcards.Api.BL.Facades
{
    public interface IUserfacade : IFacade
    {
        public Guid UserCreate(UserDetailModel user);

        public Guid CreateOrUpdate(UserDetailModel user);

        public void DeleteUser(UserDetailModel user);

        public List<UserListModel> GetAll();

        public UserDetailModel? GetById(Guid id);

        public Guid? UserUpdate(UserDetailModel user);
    }
}



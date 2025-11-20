using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;


namespace Flashcards.Api.BL.Facades
{
    public interface IUserFacade : IFacade
    {
        public Guid Create(UserDetailModel user);

        public Guid CreateOrUpdate(UserDetailModel user);

        public void Delete(Guid id);

        public List<UserListModel> GetAll();

        public UserDetailModel? GetById(Guid id);

        public Guid? Update(UserDetailModel user);
    }
}



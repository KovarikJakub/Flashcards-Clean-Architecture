using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;


namespace Flashcards.Api.BL.Facades
{
    public interface IUserFacade : IFacade
    {
        List<UserListModel> GetAll();
        UserDetailModel? GetById(Guid id);
        Guid CreateOrUpdate(UserDetailModel UserModel, IList<string> userRoles, string? ownerId = null);
        Guid Create(UserDetailModel UserModel, IList<string> userRoles, string? ownerId = null);
        Guid? Update(UserDetailModel UserModel, IList<string> userRoles, string? ownerId = null);
        void Delete(Guid id, IList<string> userRoles, string? ownerId = null);
    }
}



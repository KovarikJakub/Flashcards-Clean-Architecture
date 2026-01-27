using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface IUserFacade : IFacade
    {
        List<UserListModel> GetAll();
        List<UserListModel> GetByQuery(UserQueryModel query);
        UserDetailModel? GetById(Guid id);
        UserDetailModel? GetByIdentityId(string identityId);
        Guid Create(UserCreateModel user, IList<string> userRoles, string requestorId);
        Guid? Update(UserDetailModel user, IList<string> userRoles, string requestorId);
        void Delete(Guid id, IList<string> userRoles, string requestorId);
    }
}

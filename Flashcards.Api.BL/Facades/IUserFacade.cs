using Flashcards.Common.BL.Facades;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;


namespace Flashcards.Api.BL.Facades
{
    public interface IUserFacade : IFacade
    {
        List<UserListModel> GetAll();
        List<UserListModel> GetByQuery(UserQueryModel query);
        UserDetailModel? GetById(Guid id);
        Guid CreateOrUpdate(UserDetailModel UserModel, IList<string> userRoles, string? ownerId = null);
        Guid Create(UserDetailModel UserModel, IList<string> userRoles, string? ownerId = null);
        Guid? Update(UserDetailModel UserModel, IList<string> userRoles, string? ownerId = null);
        void Delete(Guid id, IList<string> userRoles, string? ownerId = null);
        
        Task<Guid> CreateUserWithIdentityAsync(
            string userName,
            string password,
            string email,
            string name,
            string? photographyUrl,
            RoleType role,
            IList<string> userRoles,
            string? ownerId = null);
        
        Task DeleteUserWithIdentityAsync(Guid id, IList<string> userRoles, string? ownerId = null);
        
        UserDetailModel? GetByOwnerId(string ownerId);
    }
}

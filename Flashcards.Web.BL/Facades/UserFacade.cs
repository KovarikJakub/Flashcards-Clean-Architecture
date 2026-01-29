using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Extensions;

namespace Flashcards.Web.BL.Facades
{
    public class UserFacade(IUserApiClient apiClient, HttpClient httpClient)
    {
        public async Task<UserDetailModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await apiClient.UserGetAsync(id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<UserListModel>> GetAllAsync()
        {
            var users = await apiClient.UserGetAsync();
            return users.ToList();
        }

        public async Task<UserDetailModel> GetByIdentityIdAsync(string id)
        {
            return await apiClient.IdentityAsync(id);
        }

        public async Task<Guid?> Update(UserDetailModel user)
        {
            return await apiClient.UserPutAsync(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            await apiClient.UserDeleteAsync(id);
        }

    }
}

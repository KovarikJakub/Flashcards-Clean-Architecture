using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Extensions;

namespace Flashcards.Web.BL.Facades
{
    public class UserFacade
    {
        private readonly IUserApiClient apiClient;

        public UserFacade(IUserApiClient apiClient)
        {
            this.apiClient = apiClient;
        }
        
        public async Task<UserDetailModel?> GetByOwnerIdAsync(string ownerId)
        {
            try
            {
                var user = await apiClient.OwnerAsync(ownerId);
                return user;
            }
            catch
            {
                return null;
            }
        }

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

        public async Task DeleteAsync(Guid id)
        {
            await apiClient.UserDeleteAsync(id);
        }

        public async Task<Guid> UpsertAsync(UserDetailModel model)
        {
            var id = await apiClient.UpsertAsync(model);
            return id;
        }
    }
}

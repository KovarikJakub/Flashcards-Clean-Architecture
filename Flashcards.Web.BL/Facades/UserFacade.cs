using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Extensions;

namespace Flashcards.Web.BL.Facades
{
    public class UserFacade
    {
        private readonly IUserApiClient apiClient;
        private readonly HttpClient httpClient;

        public UserFacade(IUserApiClient apiClient, HttpClient httpClient)
        {
            this.apiClient = apiClient;
            this.httpClient = httpClient;
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

        /// <summary>
        /// Creates a new user in both Identity Server and API
        /// Uses the authenticated HttpClient from IUserApiClient's configuration
        /// </summary>
        public async Task<Guid> CreateUserWithIdentityAsync(
            string userName,
            string password,
            string email,
            string name,
            string? photographyUrl,
            RoleType role,
            CancellationToken cancellationToken = default)
        {
            return await apiClient.CreateUserWithIdentityAsync(
                httpClient,
                userName,
                password,
                email,
                name,
                photographyUrl,
                role,
                cancellationToken);
        }

        /// <summary>
        /// Deletes user from both Identity Server and API
        /// </summary>
        public async Task DeleteUserWithIdentityAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await apiClient.DeleteUserWithIdentityAsync(httpClient, id, cancellationToken);
        }
    }
}

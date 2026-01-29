
namespace Flashcards.Api.BL.Services
{
    public interface IIdentityService
    {
        Task<string> CreateIdentityAsync(string username, string password, string role);
        Task DeleteIdentityAsync(string userId);
    }
}


namespace Flashcards.Api.BL.Services
{
    public interface IIdentityService
    {
        Task<string> CreateIdentityAsync(string username, string password);
        Task DeleteIdentityAsync(string userId);
    }
}

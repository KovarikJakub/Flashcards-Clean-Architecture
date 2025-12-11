using Flashcards.Common.Models;
using Flashcards.Web.BL.Api;

namespace Flashcards.Web.BL.Facades
{
    public class LessonFacade
    {
            private readonly ILessonApiClient _apiClient;
            
            public LessonFacade(ILessonApiClient apiClient)
            {
                _apiClient = apiClient;
            }

            public async Task<List<LessonListModel>> GetUserHistoryAsync(string userId)
            {
                var result = await _apiClient.UserAsync(userId);
                return result.ToList();
            }

            public async Task <ICollection<LessonListModel>> GetAllAsync()
            {
                var lessons = await _apiClient.LessonGetAsync();
                return lessons.ToList();
            }
            
            public async Task<LessonDetailModel?> GetByIdAsync(Guid id)
            {
                try
                {
                    var lesson = await _apiClient.LessonGetAsync(id);
                    return lesson;
                }
                catch
                {
                    return null;
                }
            }
            
            public async Task DeleteAsync(Guid id)
            {
                await _apiClient.LessonDeleteAsync(id);
            }
            
            public async Task<Guid> UpsertAsync(LessonDetailModel model)
            {
                return await _apiClient.LessonPostAsync(model);
            }
    }   
}

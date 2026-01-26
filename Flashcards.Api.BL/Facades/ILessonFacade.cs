using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ILessonFacade : IFacade
    {
        public LessonDetailModel? GetById(Guid id);
        public List<LessonListModel> GetAll();
        List<LessonListModel> GetByQuery(LessonQueryModel query);
        public Guid Create(LessonCreateModel model, IList<string> userRoles, string requestorId);
        public void Delete(Guid id, IList<string> userRoles, string requestorId);
    }
}

using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ILessonFacade : IFacade
    {
        public Guid Create(LessonDetailModel model);
        public LessonDetailModel? GetById(Guid id);
        public List<LessonListModel> GetAll();
        public List<LessonListModel> GetAllByOwner(string ownerId);
        public void Delete(Guid id);
        List<LessonListModel>? GetByQuery(LessonQueryModel query);
    }
}

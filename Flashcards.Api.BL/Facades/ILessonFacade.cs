using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ILessonFacade : IFacade
    {
        public Guid Create(LessonCreateModel model, string requestorId);
        public LessonDetailModel? GetById(Guid id);
        public List<LessonListModel> GetAll();
        public void Delete(Guid id);
        List<LessonListModel> GetByQuery(LessonQueryModel query);
    }
}

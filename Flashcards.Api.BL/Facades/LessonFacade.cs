using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class LessonFacade(LessonMapper mapper, ILessonRepository lessonRepository, IUserRepository userRepository, ICardCollectionRepository cardCollectionRepository) 
        : FacadeBase<ILessonRepository, LessonEntity>(lessonRepository, userRepository),
        ILessonFacade
    {
        public Guid Create(LessonCreateModel model, IList<string> userRoles, string requestorId)
        {
            var collection = cardCollectionRepository.GetById(model.CardCollectionId);
            if(collection is null)
                throw new ArgumentException("Existing collection ID is required to create a lesson.");

            var requestorUserEntity = userRepository.GetByIdentityId(requestorId);

            var entity = mapper.ToEntity(model, requestorUserEntity, collection);
            return lessonRepository.Insert(entity);
        }

        public LessonDetailModel? GetById(Guid id)
        {
            var result = lessonRepository.GetById(id);
            if (result == null) return null;
            return mapper.ToDetailModel(result);
        }

        public List<LessonListModel> GetAll()
        {
            var entities = lessonRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public void Delete(Guid id, IList<string> userRoles, string requestorId)
        {
            lessonRepository.Remove(id);
        }

        public List<LessonListModel> GetByQuery(LessonQueryModel queryModel)
        {
            var query = lessonRepository.GetAll().AsEnumerable();

            query = query.Where(lesson => lesson.User.Id == queryModel.UserId);

            if (queryModel.CardCollectionId is not null)
            {
                query = query.Where(lesson => lesson.CardCollection.Id == queryModel.CardCollectionId);
            }

            var page = queryModel.Page;
            var pageSize = queryModel.PageSize;
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return mapper.ToListModel(items);
        }
    }
}

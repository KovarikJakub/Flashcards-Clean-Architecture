
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class LessonFacade : ILessonFacade
    {
        private readonly LessonMapper _mapper;
        private readonly ILessonRepository _repository;
        public LessonFacade(LessonMapper mapper, ILessonRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Guid Create(LessonDetailModel model)
        {
            return _repository.Insert(_mapper.ToEntity(model));
        }

        public LessonDetailModel? GetById(Guid id)
        {
            var result = _repository.GetById(id);
            if (result == null) return null;
            return _mapper.ToDetailModel(result);
        }

        public List<LessonListModel> GetAll()
        {
            var entities = _repository.GetAll();
            return _mapper.ToListModel(entities);
        }

        public void Delete(Guid id)
        {
            _repository.Remove(id);
        }

        public List<LessonListModel>? GetByQuery(LessonQueryModel queryModel)
        {
            var query = _repository.GetAll().AsEnumerable();

            if (queryModel.UserId is not null)
            {
                query = query.Where(lesson => lesson.User.Id == queryModel.UserId);
            }

            if (queryModel.CardCollectionId is not null)
            {
                query = query.Where(lesson => lesson.CardCollection.Id == queryModel.CardCollectionId);
            }

            var page = queryModel.Page;
            var pageSize = queryModel.PageSize;
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return _mapper.ToListModel(items);
        }
    }
}

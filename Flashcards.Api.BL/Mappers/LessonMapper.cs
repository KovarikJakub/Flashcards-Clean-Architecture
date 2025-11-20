using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;
using System.Reflection;

namespace Flashcards.Api.BL.Mappers
{
    public class LessonMapper(UserMapper userMapper, CardCollectionMapper collectionMapper)
    {
        public LessonEntity ToEntity(LessonDetailModel model)
        {
            return new LessonEntity
            {
                Id = model.Id,
                Start = model.Start,
                Correct = model.Correct,
                Seen = model.Seen,
                User = userMapper.ToEntity(model.User),
                CardCollection = collectionMapper.ToEntity(model.CardCollection),
                Wrong = model.Wrong
            };
        }

        public LessonDetailModel ToDetailModel(LessonEntity entity)
        {
            return new LessonDetailModel
            {
                Id = entity.Id,
                Start = entity.Start,
                Wrong = entity.Wrong,
                Seen = entity.Seen,
                Correct = entity.Correct,
                User = userMapper.ToDetailModel(entity.User),
                CardCollection = collectionMapper.ToDetailModel(entity.CardCollection)
            };
        }

        public List<LessonListModel> ToListModel(IEnumerable<LessonEntity> entities)
        {
            return entities.Select(e => new LessonListModel
            {
                Id = e.Id,
                Start = e.Start,
                CardCollectionName = e.CardCollection.Title,
                CardCount = e.CardCollection.Cards.Count,
                Wrong = e.Wrong,
                Correct = e.Correct,
                Seen = e.Seen
            }).ToList();
        }
    }
}

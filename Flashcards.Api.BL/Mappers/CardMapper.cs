namespace Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models.Card;

public class CardMapper
{
    public CardEntity ToEntity(CardDetailModel model)
    {
        return new CardEntity
        {
            Id = model.Id,
            QuestionType = model.QuestionType,
            AnswerType = model.AnswerType,
            QuestionContent = model.QuestionContent,
            AnswerContent = model.AnswerContent,
            DateCreated = model.DateCreated,
            Description = model.Description
        };
    }

    public CardDetailModel ToDetailModel(CardEntity entity)
    {
        return new CardDetailModel
        {
            Id = entity.Id,
            QuestionType = entity.QuestionType,
            AnswerType = entity.AnswerType,
            QuestionContent = entity.QuestionContent,
            AnswerContent = entity.AnswerContent,
            DateCreated = entity.DateCreated,
            Description = entity.Description
        };
    }

    public List<CardListModel> ToListModel(IEnumerable<CardEntity> entities)
    {
        return entities.Select(e => new CardListModel
        {
            Id = e.Id,
            QuestionContent = e.QuestionContent,
            Description = e.Description
        }).ToList();
    }
}
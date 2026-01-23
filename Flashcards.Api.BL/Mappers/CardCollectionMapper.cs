using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Mappers;
public class CardCollectionMapper
{
    public CardCollectionEntity ToEntity(CardCollectionDetailModel model, string? ownerId = null)
    {
        return new CardCollectionEntity
        {
            Id = model.Id,
            Title = model.Title,
            AnswerStart = model.AnswerStart,
            AnswerEnd = model.AnswerEnd,
            Cards = model.Cards            
            .Select(card => new CardEntity
            {
                Id = card.Id,
                QuestionType = card.QuestionType,
                AnswerType = card.AnswerType,
                QuestionContent = card.QuestionContent,
                AnswerContent = card.AnswerContent,
                Created = card.DateCreated,
                Description = card.Description
            })
            .ToList()
        };
    }

    public CardCollectionDetailModel ToDetailModel(CardCollectionEntity model)
    {
        return new CardCollectionDetailModel
        {
            Id = model.Id,
            Title = model.Title,
            AnswerStart = model.AnswerStart,
            AnswerEnd = model.AnswerEnd,
            Cards = model.Cards
            .Select(card => new CardDetailModel
            {
                Id = card.Id,
                QuestionType = card.QuestionType,
                AnswerType = card.AnswerType,
                QuestionContent = card.QuestionContent,
                AnswerContent = card.AnswerContent,
                DateCreated = card.Created,
                Description = card.Description


            })
            .ToList()

        };
    }

    public List<CardCollectionListModel> ToListModel(IEnumerable<CardCollectionEntity> entities)
    {
        return entities.Select(e => new CardCollectionListModel
        {
            Id = e.Id,
            Title = e.Title,
            AnswerStart = e.AnswerStart,
            AnswerEnd = e.AnswerEnd

        }).ToList();
    }
}
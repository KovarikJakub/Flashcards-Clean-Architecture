using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Entities.Interfaces;
using Flashcards.Common.Models;
using System.Reflection;

namespace Flashcards.Api.BL.Mappers;
public class CardCollectionMapper
{
    public CardCollectionEntity ToEntity(CardCollectionDetailModel model)
    {
        return new CardCollectionEntity
        {
            Id = model.Id,
            Title = model.Title,
            AnswerStart = model.AnswerStart,
            AnswerEnd = model.AnswerEnd,
            OwnerId = model.OwnerId,
            Cards = model.Cards            
            .Select(card => new CardEntity
            {
                Id = card.Id,
                QuestionType = card.QuestionType,
                AnswerType = card.AnswerType,
                QuestionContent = card.QuestionContent,
                AnswerContent = card.AnswerContent,
                DateCreated = card.DateCreated,
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
            OwnerId = model.OwnerId,
            Cards = model.Cards
            .Select(card => new CardDetailModel
            {
                Id = card.Id,
                QuestionType = card.QuestionType,
                AnswerType = card.AnswerType,
                QuestionContent = card.QuestionContent,
                AnswerContent = card.AnswerContent,
                DateCreated = card.DateCreated,
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
            AnswerEnd = e.AnswerEnd,
            OwnerId = e.OwnerId

        }).ToList();
    }
}
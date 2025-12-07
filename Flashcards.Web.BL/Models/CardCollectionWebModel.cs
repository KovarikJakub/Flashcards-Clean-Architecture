using System;
using Flashcards.Common.Models;

namespace Flashcards.Web.BL.Models
{
    /// <summary>
    /// Web-only wrapper that carries the API model plus a temporary OwnerId.
    /// This keeps the extra owner info confined to the Web layer until the backend is fixed.
    /// </summary>
    public class CardCollectionWebModel
    {
        public CardCollectionDetailModel ApiModel { get; set; }

        /// <summary>
        /// Temporary owner id stored only in web layer.
        /// </summary>
        public Guid? OwnerId { get; set; }

        public CardCollectionWebModel()
        {
            ApiModel = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = string.Empty,
                AnswerStart = DateTime.Now,
                AnswerEnd = DateTime.Now.AddHours(1)
            };
        }

        public CardCollectionWebModel(CardCollectionDetailModel apiModel, Guid? ownerId = null)
        {
            ApiModel = apiModel ?? new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = string.Empty,
                AnswerStart = DateTime.Now,
                AnswerEnd = DateTime.Now.AddHours(1)
            };
            OwnerId = ownerId;
        }
    }
}

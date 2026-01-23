
using System.Net.Mime;

namespace Flashcards.Common.Models
{
    public class CardQueryModel : BaseQueryModel
    {
        public string? SearchString;
        public ContentType? QuestionType;
        public ContentType? AnswerType;
    }
}

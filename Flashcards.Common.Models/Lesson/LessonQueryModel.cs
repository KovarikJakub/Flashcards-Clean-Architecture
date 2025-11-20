
namespace Flashcards.Common.Models
{
    public class LessonQueryModel : BaseQueryModel
    {
        public Guid? UserId { get; set; } = null;
        public Guid? CardCollectionId { get; set; } = null;
    }
}

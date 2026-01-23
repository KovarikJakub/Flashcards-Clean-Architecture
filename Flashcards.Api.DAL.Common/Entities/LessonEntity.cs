namespace Flashcards.Api.DAL.Common.Entities
{
    public record LessonEntity : EntityBase
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public Guid CardCollectionId { get; set; }
        public CardCollectionEntity? CardCollection { get; set; }
        public required DateTime Start { get; set; }
        public required uint Seen { get; set; }
        public required uint Correct { get; set; }
        public required uint Wrong { get; set; }
    }
}
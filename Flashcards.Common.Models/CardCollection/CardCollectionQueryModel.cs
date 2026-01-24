
namespace Flashcards.Common.Models
{
    public class CardCollectionQueryModel : BaseQueryModel
    {
        public string? SearchString { get; set; }
        public Guid? OwnerId { get; set; }
        public bool IncludeMine { get; set; } = true;
        public bool IncludeOthers { get; set; } = true;
        public bool ShowCurrentlyAvailableOnly { get; set; } = false;
    }
}

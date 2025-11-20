namespace Flashcards.Web.BL.Api
{
    public partial class CardCollectionApiClient
    {
        public CardCollectionApiClient(HttpClient httpClient, string baseUrl) : this(httpClient)
        {
            BaseUrl = baseUrl;
        }
    }
}

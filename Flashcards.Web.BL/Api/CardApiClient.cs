namespace Flashcards.Web.BL.Api
{
    public partial class CardApiClient
    {
        public CardApiClient(HttpClient httpClient, string baseUrl) : this(httpClient)
        {
            BaseUrl = baseUrl;
        }
    }
}

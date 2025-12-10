namespace Flashcards.Web.BL.Api
{ 
    public partial class LessonApiClient
    {
        public LessonApiClient(HttpClient httpClient, string baseUrl) : this(httpClient)
        {
            BaseUrl = baseUrl;
        }
    }   
}


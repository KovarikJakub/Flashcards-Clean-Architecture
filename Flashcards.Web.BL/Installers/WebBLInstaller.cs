using Flashcards.Common.Options;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Facades;
using Flashcards.Web.BL.MessageHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Flashcards.Web.BL.Installers;

public class WebBLInstaller
{
    public void Install(
        IServiceCollection serviceCollection,
        IdentityOptions identityOptions,
        ApiOptions apiOptions)
    {
        // Message handler provides token or fallback behaviour
        serviceCollection.AddScoped<CustomAuthorizationMessageHandler>();

        // Register API client using HttpClientFactory for card collections
        var cardClientBuilder = serviceCollection.AddHttpClient<ICardCollectionApiClient, CardCollectionApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiOptions.BaseUrl);
        });

        cardClientBuilder.AddHttpMessageHandler(serviceProvider =>
            serviceProvider.GetRequiredService<CustomAuthorizationMessageHandler>()
                .ConfigureHandler(authorizedUrls: new[] { apiOptions.BaseUrl }, scopes: identityOptions.DefaultScopes));

        // Register API client using HttpClientFactory for users
        var userClientBuilder = serviceCollection.AddHttpClient<IUserApiClient, UserApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiOptions.BaseUrl);
        });

        userClientBuilder.AddHttpMessageHandler(serviceProvider =>
            serviceProvider.GetRequiredService<CustomAuthorizationMessageHandler>()
                .ConfigureHandler(authorizedUrls: new[] { apiOptions.BaseUrl }, scopes: identityOptions.DefaultScopes));

        // Register facades and other services explicitly
        serviceCollection.AddTransient<CardCollectionFacade>();
        serviceCollection.AddTransient<UserFacade>();
    }
}

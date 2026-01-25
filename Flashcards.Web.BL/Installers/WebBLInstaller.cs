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
        serviceCollection.AddScoped<CustomAuthorizationMessageHandler>();

        AddApiClient<ICardCollectionApiClient, CardCollectionApiClient>(serviceCollection, identityOptions, apiOptions);
        AddApiClient<IUserApiClient, UserApiClient>(serviceCollection, identityOptions, apiOptions);
        AddApiClient<ILessonApiClient, LessonApiClient>(serviceCollection, identityOptions, apiOptions);
        AddApiClient<ICardApiClient, CardApiClient>(serviceCollection, identityOptions, apiOptions);

        serviceCollection.AddTransient<CardCollectionFacade>();
        serviceCollection.AddTransient<LessonFacade>();
        serviceCollection.AddTransient<CardFacade>();
        
        // UserFacade 
        serviceCollection.AddTransient<UserFacade>(sp =>
        {
            var userApiClient = sp.GetRequiredService<IUserApiClient>();
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IUserApiClient));
            return new UserFacade(userApiClient, httpClient);
        });
    }

    private void AddApiClient<TInterface, TImplementation>(
        IServiceCollection serviceCollection,
        IdentityOptions identityOptions,
        ApiOptions apiOptions)
        where TInterface : class, IApiClient
        where TImplementation : class, TInterface
    {
        var httpClientBuilder = serviceCollection.AddHttpClient<TInterface, TImplementation>(client =>
        {
            client.BaseAddress = new Uri(apiOptions.BaseUrl);
        });

        if (identityOptions.IsIdentityEnabled)
        {
            httpClientBuilder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        }
    }
}
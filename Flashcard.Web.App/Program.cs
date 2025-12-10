using Flashcards.Web.App;
using Flashcards.Web.BL.Api;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.json");
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl")!;

builder.Services.AddTransient<ICardCollectionApiClient, CardCollectionApiClient>(provider =>
{
    var client = new HttpClient() { BaseAddress = new Uri(apiBaseUrl) };
    return new CardCollectionApiClient(client, apiBaseUrl);
});

builder.Services.AddTransient<ICardApiClient, CardApiClient>(provider =>
{
    var client = new HttpClient() { BaseAddress = new Uri(apiBaseUrl) };
    return new CardApiClient(client, apiBaseUrl);
});

builder.Services.AddTransient<ILessonApiClient, LessonApiClient>(provider =>
{
    var client = new HttpClient() { BaseAddress = new Uri(apiBaseUrl) };
    return new LessonApiClient(client, apiBaseUrl);
});

builder.Services.AddTransient<CardCollectionFacade>();
builder.Services.AddTransient<CardFacade>();
builder.Services.AddTransient<LessonFacade>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

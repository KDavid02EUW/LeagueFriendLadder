using LeagueFriendLadder;
using LeagueFriendLadder.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<PlayerSessionService>();
builder.Services.AddScoped<RiotService>();
builder.Services.AddScoped<ApiService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7180/")
});

builder.Services.AddHttpClient("RiotClient", client =>
{
    client.BaseAddress = new Uri("https://europe.api.riotgames.com/");
});

await builder.Build().RunAsync();
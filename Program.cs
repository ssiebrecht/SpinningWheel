using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpinningWheel;
using SpinningWheel.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<INameStore, NameStore>();
builder.Services.AddScoped<ISpinService, SpinService>();
builder.Services.AddScoped<ISpinBroadcaster, SpinBroadcaster>();
builder.Services.AddScoped<ISpotifyPlayer, SpotifyPlayer>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

await builder.Build().RunAsync();

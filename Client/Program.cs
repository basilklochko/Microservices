using Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAntDesign();

// builder.Services.AddSingleton<HubConnection>(sp =>
// {
//     // var navigationManager = sp.GetRequiredService<NavigationManager>();
//     return new HubConnectionBuilder()
//         .WithUrl("http://localhost:5069/booking")
//         .WithAutomaticReconnect()
//         .WithUrl("http://localhost:5064/air")
//         .WithAutomaticReconnect()
//         .Build();
// });

CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

await builder.Build().RunAsync();

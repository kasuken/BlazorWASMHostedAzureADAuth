using BlazorWASMSecurityAzureAD.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BlazorWASMSecurityAzureAD.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWASMSecurityAzureAD.ServerAPI"));

builder.Services.AddMsalAuthentication<RemoteAuthenticationState, SecureUserAccount>(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://957e1304-dd53-4589-b013-4a264dd19334/BlazorWASMHosted.API");
    options.UserOptions.RoleClaim = "appRole";
})
.AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, SecureUserAccount,
    SecureAccountFactory>();

await builder.Build().RunAsync();

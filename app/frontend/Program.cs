// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(nameof(AppSettings)));
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddScoped<OpenAIPromptQueue>();
builder.Services.AddLocalStorageServices();
builder.Services.AddSessionStorageServices();
builder.Services.AddSpeechSynthesisServices();
builder.Services.AddSpeechRecognitionServices();
builder.Services.AddMudServices();

await JSHost.ImportAsync(
    moduleName: nameof(JavaScriptModule),
    moduleUrl: $"../js/iframe.js?{Guid.NewGuid()}" /* cache bust */);

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://56dc48e0-5eba-4769-bfeb-9a2d0192ebe0/API.Access");
});

var host = builder.Build();
await host.RunAsync();

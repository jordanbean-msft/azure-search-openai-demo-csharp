// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureAzureKeyVault();

// See: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOutputCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//builder.Services.AddAuthorizationBuilder().AddPolicy("AzureAd", policy => policy.RequireScope("API.Access"));
builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("AzureAd", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scp", "API.Access");
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddCrossOriginResourceSharing();
builder.Services.AddAzureServices();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        var name = builder.Configuration["AzureRedisCacheName"] +
			".redis.cache.windows.net" ;
        var key = builder.Configuration["AzureRedisCachePrimaryKey"];
		var ssl = "true";

        string? GetEnvVar(string key) =>
            Environment.GetEnvironmentVariable(key);

		if (GetEnvVar("REDIS_HOST") is string redisHost)
		{
			name = $"{redisHost}:{GetEnvVar("REDIS_PORT")}";                
			key = GetEnvVar("REDIS_PASSWORD");
			ssl = "false";
		}

		if (GetEnvVar("AZURE_REDIS_HOST") is string azureRedisHost)
		{
			name = $"{azureRedisHost}:{GetEnvVar("AZURE_REDIS_PORT")}";
			key = GetEnvVar("AZURE_REDIS_PASSWORD");
			ssl = "false";
		}

        options.Configuration = $"""
            {name},abortConnect=false,ssl={ssl},allowAdmin=true,password={key}
            """;
        options.InstanceName = "content";
    });
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseCors();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapSwagger();
app.MapApi();

app.Run();

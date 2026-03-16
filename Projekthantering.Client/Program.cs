using Microsoft.AspNetCore.Components.Authorization;
using Projekthantering.Client.Components;
using Projekthantering.Client.Services;

namespace Projekthantering.Client;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Auth
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<AuthStateProvider>());

        // HttpClient mot API
        builder.Services.AddScoped<AuthHeaderHandler>();
        builder.Services.AddHttpClient("API", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7191");
        }).AddHttpMessageHandler<AuthHeaderHandler>();
        builder.Services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

        // Services
        builder.Services.AddScoped<AuthService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found",
            createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}

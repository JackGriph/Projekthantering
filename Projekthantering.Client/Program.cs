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
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<TokenProvider>();
        builder.Services.AddScoped<AuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<AuthStateProvider>());

        // HttpClient mot API – skapas direkt i circuit-scopen
        // (IHttpClientFactory pooler handlers separat, vilket bryter scoped TokenProvider)
        builder.Services.AddScoped(sp =>
        {
            var tokenProvider = sp.GetRequiredService<TokenProvider>();
            var handler = new AuthHeaderHandler(tokenProvider)
            {
                InnerHandler = new HttpClientHandler()
            };
            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7191")
            };
        });

        // Services
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IBoardService, BoardService>();
        builder.Services.AddScoped<IListService, ListService>();
        builder.Services.AddScoped<ICardService, CardService>();

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
            .AddInteractiveServerRenderMode()
            .AllowAnonymous();

        app.Run();
    }
}

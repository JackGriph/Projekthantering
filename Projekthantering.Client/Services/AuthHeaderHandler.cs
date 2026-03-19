using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Projekthantering.Client.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ProtectedLocalStorage _localStorage;

    public AuthHeaderHandler(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _localStorage.GetAsync<string>("accessToken");
            if (!string.IsNullOrEmpty(result.Value))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Value);
            }
        }
        catch (InvalidOperationException)
        {
            // JS interop not available during prerendering
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

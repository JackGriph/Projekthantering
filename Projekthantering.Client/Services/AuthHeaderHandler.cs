using System.Net.Http.Headers;

namespace Projekthantering.Client.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly TokenProvider _tokenProvider;

    public AuthHeaderHandler(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

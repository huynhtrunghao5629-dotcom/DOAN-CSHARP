using System.Net.Http.Headers;
using System.Security.Claims;

namespace QuanLyKhachSan.Web.Services;

// Forward the API token only in server-side calls from the authenticated Web session.
public sealed class CustomerApiTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CustomerApiTokenHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _contextAccessor.HttpContext?.User.FindFirst("SawadeApiToken")?.Value;
        if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization == null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}

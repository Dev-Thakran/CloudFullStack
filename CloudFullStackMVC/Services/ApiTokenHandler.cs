using System.Net.Http.Headers;

namespace CloudFullStackMVC.Services
{
    // Runs before every request sent by the "SpotSafeApi" HttpClient.
    // Reads this user's token from the Session and adds the Bearer header.
    public class ApiTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = _httpContextAccessor.HttpContext?.Session.GetString("ApiToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
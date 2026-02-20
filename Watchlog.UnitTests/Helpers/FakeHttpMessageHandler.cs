using System.Net;
using System.Net.Http;
using System.Text;

namespace Watchlog.UnitTests.Helpers
{
    public sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_handler(request));

        public static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK)
            => new(status)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
    }
}
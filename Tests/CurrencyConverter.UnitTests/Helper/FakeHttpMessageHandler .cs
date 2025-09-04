using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverter.Tests.Unit.Helpers
{
    public sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public int CallCount { get; private set; }

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            var response = _responder(request);
            return Task.FromResult(response);
        }

        public static HttpResponseMessage Json(string json, HttpStatusCode code = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(code)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eventstore.Client.UnitTest
{
    public class TestingHttpClient : HttpClient
    {
        private readonly TestingHttpHandler _handler;

        public TestingHttpClient()
        : this(new TestingHttpHandler())
        {
            
        }

        internal TestingHttpClient(TestingHttpHandler handler)
            : base(handler)
        {
            _handler = handler;
        }

        public void RespondWith(HttpResponseMessage message)
        {
            _handler.RespondWith(message);
        }
    }


    internal class TestingHttpHandler : HttpClientHandler
    {
        private TaskCompletionSource<HttpResponseMessage> _completion;

        public TestingHttpHandler()
        {

        }

        public void RespondWith(HttpResponseMessage message)
        {
            _completion.SetResult(message);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _completion = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(() => _completion.TrySetCanceled()))
            {
                return await _completion.Task.ConfigureAwait(false);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorBelt.Tooling
{
    public class DefaultHttpClient : IHttpClient
    {

        private HttpClient _client = new HttpClient();

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "es_admin", "solomotoUpsourceP@sswd"))));
            return _client.PostAsync(requestUri, content);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;

namespace ConveyorBelt.Tooling
{
    public class ElasticsearchClient : IElasticsearchClient
    {

        private HttpClient _httpClient = new HttpClient();
        private const string IndexFormat = "{0}/{1}";
        private const string IndexSearchFormat = "{0}/{1}/_search?size=0";
        private const string MappingFormat = "{0}/{1}/{2}/_mapping";


        public async Task<bool> CreateIndexIfNotExistsAsync(string baseUrl, string indexName)
        {
            baseUrl = baseUrl.TrimEnd('/');
            string searchUrl = string.Format(IndexSearchFormat, baseUrl, indexName);
            //_httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + "ZXNfYWRtaW46c29sb21vdG9VcHNvdXJjZVBAc3N3ZA==");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "es_admin", "solomotoUpsourceP@sswd"))));
            var response = await _httpClient.GetAsync(searchUrl);
            
            var text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return false;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var url = string.Format(IndexFormat, baseUrl, indexName);
                var result = await _httpClient.PutAsJsonAsync(url, string.Empty);
                result.EnsureSuccessStatusCode();
                return true;
            }
            else
            {
                throw new ApplicationException(string.Format("Error {0}: {1}",
                    response.StatusCode,
                    text));
            }
        }

        public async Task<bool> MappingExistsAsync(string baseUrl, string indexName, string typeName)
        {
            baseUrl = baseUrl.TrimEnd('/');
            var url = string.Format(MappingFormat, baseUrl, indexName, typeName);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "es_admin", "solomotoUpsourceP@sswd"))));
            var response = await _httpClient.GetAsync(url);
            var text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && text != "{}")
            {
                return true;
            }
            if (response.StatusCode == HttpStatusCode.NotFound || text == "{}")
            {
                return false;
            }
            else
            {
                throw new ApplicationException(string.Format("Error {0}: {1}",
                    response.StatusCode,
                    text));
            }
        }

        public async Task<bool> UpdateMappingAsync(string baseUrl, string indexName, string typeName, string mapping)
        {
            baseUrl = baseUrl.TrimEnd('/');
            var url = string.Format(MappingFormat, baseUrl, indexName, typeName);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "es_admin", "solomotoUpsourceP@sswd"))));
            var response = await _httpClient.PutAsync(url, 
                new StringContent(mapping, Encoding.UTF8, "application/json"));
            var text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new ApplicationException(string.Format("Error {0}: {1}",
                    response.StatusCode,
                    text));
            }
        }

    }
}

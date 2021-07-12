using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sympli.Api
{
    public interface IHttpRequestHandler
    {
        Task<string> GetStringAsync(string requestUrl, Dictionary<string, string> requestHeaders = null);
    }

    public class HttpRequestHandler : IHttpRequestHandler
    {
        private readonly HttpClient _httpClient;
        public HttpRequestHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStringAsync(string requestUrl, Dictionary<string, string> requestHeaders = null)
        {
            if(requestHeaders != null)
            {
                requestHeaders.Union(GetDefaultHeaders());
            }
            else
            {
                requestHeaders = GetDefaultHeaders();                
            }

            foreach (var item in requestHeaders)
            {
                _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

            return await _httpClient.GetStringAsync(requestUrl);
        }

        private Dictionary<string,string> GetDefaultHeaders()
        {
            return new Dictionary<string, string> { { "User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36" } };
        }
    }
}

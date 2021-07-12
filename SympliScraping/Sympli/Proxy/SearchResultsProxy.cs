using Sympli.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sympli.Proxy
{
    public interface ISearchResultsProxy
    {
        Task<List<string>> GetScrapeResultsAsync(string searchUrl);
    }

    public class SearchResultsProxy : ISearchResultsProxy
    {
        private readonly IHttpRequestHandler _httpRequestHandler;
        public SearchResultsProxy(IHttpRequestHandler httpRequestHandler)
        {
            _httpRequestHandler = httpRequestHandler;
        }

        public async Task<List<string>> GetScrapeResultsAsync(string searchUrl)
        {
            var response = await _httpRequestHandler.GetStringAsync(searchUrl);
            var regex = new Regex("<a href=\"([^\"]+)");
            var urlsInSearchResults = regex.Matches(response).Cast<Match>().ToList();

            return urlsInSearchResults.Select(x => x.Value.ToString()).ToList();

            
        }
    }
}

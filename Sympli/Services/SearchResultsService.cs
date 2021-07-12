using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Sympli.Models;
using Sympli.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sympli.Services
{
    public interface ISearchResultsService
    {
        Task<List<SearchResultsModel>> GetScrapeResultsAsync(InputRequestModel request);
    }
    public class SearchResultsService : ISearchResultsService
    {
        private readonly ISearchResultsProxy _searchResultsProxy;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _memoryCache;
        private const int resultSize = 100;
        public SearchResultsService(ISearchResultsProxy searchResultsProxy, IConfiguration config, IMemoryCache memoryCache)
        {
            _searchResultsProxy = searchResultsProxy;
            _config = config;
            _memoryCache = memoryCache;
        }

        public  async Task<List<SearchResultsModel>> GetScrapeResultsAsync(InputRequestModel request)
        {
            var searchEngines = _config.GetSection("AppSettings").GetSection("SearchEngines").Value.ToString().Split(',').ToList();
            var searchResults = new List<SearchResultsModel>();
           
                        
            foreach (var engine in searchEngines)
            {
                SearchResultsModel _cacheEntry;
                var _cacheKey = $"{engine}search?num={resultSize}&q={request.SearchQuery}";
                var searchResult = new SearchResultsModel();
                
                if (!_memoryCache.TryGetValue(_cacheKey, out _cacheEntry))
                {
                    var result = await _searchResultsProxy.GetScrapeResultsAsync($"{engine}search?num={resultSize}&q={request.SearchQuery}");
                    var count = result.Where(x => x.ToString().Contains(request.UrlText)).ToList().Count;

                    searchResult.SearchEngine = engine;
                    searchResult.Appearences = count;

                    _cacheEntry = searchResult;
                    // Set cache options.
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        // Keep in cache for this time, reset time if accessed.
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                    // Save data in cache.
                    _memoryCache.Set(_cacheKey, _cacheEntry, cacheEntryOptions);
                }
                    
                searchResults.Add(_cacheEntry);                                                             
            }   
            
            return searchResults;
        }
    }
}

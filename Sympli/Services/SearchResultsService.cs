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
        private readonly ICacheService _cacheService;
        private const int resultSize = 100;
        public SearchResultsService(ISearchResultsProxy searchResultsProxy, IConfiguration config, ICacheService cacheService)
        {
            _searchResultsProxy = searchResultsProxy;
            _config = config;
            _cacheService = cacheService;
        }

        public  async Task<List<SearchResultsModel>> GetScrapeResultsAsync(InputRequestModel request)
        {
            var searchEngines = _config.GetSection("AppSettings").GetSection("SearchEngines").Value.ToString().Split(',').ToList();
            var searchResults = new List<SearchResultsModel>();
           
                        
            foreach (var engine in searchEngines)
            {                
                var _cacheKey = $"{engine}search?num={resultSize}&q={request.SearchQuery}";
                var _cacheEntry = _cacheService.GetCachedData<SearchResultsModel>(_cacheKey);

                if (_cacheEntry == default(SearchResultsModel))
                {
                    var result = await _searchResultsProxy.GetScrapeResultsAsync($"{engine}search?num={resultSize}&q={request.SearchQuery}");
                    var count = result.Where(x => x.ToString().Contains(request.UrlText)).ToList().Count;

                    _cacheEntry = new SearchResultsModel();
                    _cacheEntry.SearchEngine = engine;
                    _cacheEntry.Appearences = count;

                    _cacheService.SetCacheData(_cacheKey, _cacheEntry);
                }                               
                    
                searchResults.Add(_cacheEntry);                                                             
            }   
            
            return searchResults;
        }
    }
}

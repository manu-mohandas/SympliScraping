using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using Sympli.Models;
using Sympli.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    public class CacheServiceTests
    {
        private ICacheService _cacheService;
        private IMemoryCache _memoryCache;
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _configuration = Substitute.For<IConfiguration>();
            _cacheService = new CacheService(_memoryCache, _configuration);
            _configuration.GetSection("AppSettings").GetSection("CacheDuration").Value.Returns("10");
        }

        [Test]
        public void GetAndSetCacheTests()
        {
            var searchResult = new SearchResultsModel { SearchEngine = "Google", Appearences = 10 };
            _cacheService.SetCacheData("_cacheEntry", searchResult);

            var _cacheEntry = _cacheService.GetCachedData<SearchResultsModel>("_cacheEntry");

            Assert.IsNotNull(_cacheEntry);
            Assert.AreEqual("Google", _cacheEntry.SearchEngine);
            Assert.AreEqual(10, _cacheEntry.Appearences);
        }
    }
}

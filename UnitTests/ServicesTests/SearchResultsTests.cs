
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using Sympli.Api;
using Sympli.Models;
using Sympli.Proxy;
using Sympli.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ServicesTests
{
    [TestFixture]
    public class SearchResultsTests
    {
        private IHttpRequestHandler _httpRequestHandler;
        private ISearchResultsProxy _searchResultsProxy;
        private ISearchResultsService _searchResultsServices;
        private IConfiguration _config;
        private IMemoryCache _cache;

        [SetUp]
        public void SetUp()
        {
            _httpRequestHandler = Substitute.For<IHttpRequestHandler>();
            _searchResultsProxy = new SearchResultsProxy(_httpRequestHandler);
            _config = Substitute.For<IConfiguration>();
            _cache = Substitute.For<IMemoryCache>();
            _searchResultsServices = new SearchResultsService(_searchResultsProxy, _config, _cache);
        }

        [Test]
        public async Task GetScrapeResultsAsyncTests()
        {
            var inputModel = new InputRequestModel { SearchQuery = "E-Settlements", UrlText = "www.sympli.com.au" };
            var scrapeResult = File.ReadAllText(@"..\..\..\SearchResults.txt");
            _httpRequestHandler.GetStringAsync(Arg.Any<string>()).Returns(scrapeResult);
            _config.GetSection(Arg.Any<string>()).GetSection(Arg.Any<string>()).Value.Returns("https://www.google.com.au/search?num=100,https://www.yahoo.com/search?num=100");
            
            var response = await _searchResultsServices.GetScrapeResultsAsync(inputModel);

            Assert.AreEqual(5, response[0].Appearences);
            Assert.AreEqual(5, response[1].Appearences);
            Assert.AreEqual("https://www.google.com.au/search?num=100", response[0].SearchEngine);
            Assert.AreEqual("https://www.yahoo.com/search?num=100", response[1].SearchEngine);
        }
    }
}

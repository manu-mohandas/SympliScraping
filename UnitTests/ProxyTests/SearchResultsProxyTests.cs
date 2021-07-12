using NSubstitute;
using NUnit.Framework;
using Sympli.Api;
using Sympli.Proxy;
using Sympli.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ProxyTests
{
    [TestFixture]
    public class SearchResultsProxyTests
    {
        private IHttpRequestHandler _httpRequestHandler;
        private ISearchResultsProxy _searchResultsProxy;

        [SetUp]
        public void SetUp()
        {
            _httpRequestHandler = Substitute.For<IHttpRequestHandler>();
            _searchResultsProxy = new SearchResultsProxy(_httpRequestHandler);
        }

        [Test]
        public async Task GetScrapeResultsAsyncTests()
        {
            var scrapeResult = File.ReadAllText(@"..\..\..\SearchResults.txt");
            _httpRequestHandler.GetStringAsync(Arg.Any<string>()).Returns(scrapeResult);
            var response = await _searchResultsProxy.GetScrapeResultsAsync(@"https://www.google.com/search?q=e-settlements&num=100");

            Assert.AreEqual(126, response.Count);
        }
    }
}

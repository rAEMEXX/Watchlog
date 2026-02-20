using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using System.Web;
using Watchlog.Business.Options;
using Watchlog.Business.Services.Implementations;
using Watchlog.UnitTests.Helpers;
using Xunit;

namespace Watchlog.UnitTests
{
    public class TmdbServiceTests
    {
        [Fact]
        public async Task SearchMovieAsync_CallsCorrectUrl_AndParsesResults()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(req =>
            {
                req.RequestUri!.ToString().Should().Contain("search/movie");
                req.RequestUri!.ToString().Should().Contain("api_key=TESTKEY");
                req.RequestUri!.ToString().Should().Contain("query=batman");

                var json = """
                { "page": 1, "results": [ { "id": 123, "name": "ignore", "title": "Batman" } ] }
                """;
                return FakeHttpMessageHandler.Json(json);
            });

            var http = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };

            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("tmdb").Returns(http);

            var options = Options.Create(new TmdbOptions { ApiKey = "TESTKEY" });

            var service = new TmdbService(factory, options);

            // Act
            var result = await service.SearchMovieAsync("batman");

            // Assert
            result.Should().NotBeNull();
            result!.Results.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTvDetailsAsync_UsesTvEndpoint()
        {
            var handler = new FakeHttpMessageHandler(req =>
            {
                req.RequestUri!.ToString().Should().Contain("tv/777");
                req.RequestUri!.ToString().Should().Contain("api_key=TESTKEY");

                var json = """
                { "id": 777, "name": "Test Show", "first_air_date": "2020-01-01", "overview": "x", "number_of_seasons": 1, "seasons": [], "genres": [] }
                """;
                return FakeHttpMessageHandler.Json(json);
            });

            var http = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("tmdb").Returns(http);

            var options = Options.Create(new TmdbOptions { ApiKey = "TESTKEY" });
            var service = new TmdbService(factory, options);

            var result = await service.GetTvDetailsAsync(777);

            result.Should().NotBeNull();
            result!.Id.Should().Be(777);
            result.Name.Should().Be("Test Show");
        }

        [Fact]
        public async Task SearchTvAsync_EncodesQuery()
        {
            var handler = new FakeHttpMessageHandler(req =>
            {
                req.RequestUri!.AbsolutePath.Should().EndWith("/search/tv");

                var q = HttpUtility.ParseQueryString(req.RequestUri.Query);

                q["api_key"].Should().Be("TESTKEY");
                q["query"].Should().Be("mr robot"); // decoded value

                var json = """{ "page": 1, "results": [] }""";
                return FakeHttpMessageHandler.Json(json);
            });

            var http = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };

            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("tmdb").Returns(http);

            var options = Options.Create(new TmdbOptions { ApiKey = "TESTKEY" });
            var service = new TmdbService(factory, options);

            var result = await service.SearchTvAsync("mr robot");

            result.Should().NotBeNull();
        }
    }
}
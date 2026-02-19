using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Watchlog.Business.Options;
using Watchlog.Business.Services.Implementations;
using Watchlog.Models.Tmdb;
using Xunit;

namespace Watchlog.Business.Tests.Services
{
    public class TmdbServiceTests
    {
        private const string BaseUrl = "https://api.themoviedb.org/3/";
        private const string ApiKey = "test_api_key_123";

        [Fact]
        public async Task SearchTvAsync_ShouldCallExpectedUrl_AndDeserializeResults()
        {
            // Arrange
            var query = "Mr. Robot"; // tests escaping
            var expectedPathAndQuery = $"search/tv?api_key={ApiKey}&query=Mr.%20Robot";

            var responseModel = new TmdbSearchResult
            {
                Results =
                {
                    new TmdbSearchItem
                    {
                        Id = 1,
                        Name = "Mr. Robot",
                        FirstAirDate = "2015-06-24",
                        Overview = "Test overview"
                    }
                }
            };

            var handler = new CapturingJsonHandler(responseModel);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("tmdb").Returns(httpClient);

            var options = Options.Create(new TmdbOptions { ApiKey = ApiKey });

            var service = new TmdbService(factory, options);

            // Act
            var result = await service.SearchTvAsync(query);

            // Assert
            handler.LastRequestUri.Should().NotBeNull();
            handler.LastRequestUri!.ToString().Should().Be(BaseUrl + expectedPathAndQuery);

            result.Should().NotBeNull();
            result!.Results.Should().HaveCount(1);
            result.Results[0].Id.Should().Be(1);
            result.Results[0].DisplayName.Should().Be("Mr. Robot");
            result.Results[0].DisplayDate.Should().Be("2015-06-24");
        }

        [Fact]
        public async Task SearchMovieAsync_ShouldCallExpectedUrl()
        {
            // Arrange
            var query = "The Godfather";
            var expectedPathAndQuery = $"search/movie?api_key={ApiKey}&query=The%20Godfather";

            var responseModel = new TmdbSearchResult
            {
                Results =
                {
                    new TmdbSearchItem { Id = 999, Title = "The Godfather", ReleaseDate = "1972-03-14" }
                }
            };

            var handler = new CapturingJsonHandler(responseModel);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("tmdb").Returns(httpClient);

            var options = Options.Create(new TmdbOptions { ApiKey = ApiKey });

            var service = new TmdbService(factory, options);

            // Act
            var result = await service.SearchMovieAsync(query);

            // Assert
            handler.LastRequestUri.Should().NotBeNull();
            handler.LastRequestUri!.ToString().Should().Be(BaseUrl + expectedPathAndQuery);

            result.Should().NotBeNull();
            result!.Results.Should().ContainSingle();
            result.Results[0].DisplayName.Should().Be("The Godfather");
            result.Results[0].DisplayDate.Should().Be("1972-03-14");
        }

        [Fact]
        public async Task GetMovieDetailsAsync_ShouldCallExpectedUrl()
        {
            // Arrange
            var movieId = 550;
            var expectedPathAndQuery = $"movie/{movieId}?api_key={ApiKey}";

            // This model can be minimal — just ensure JSON can deserialize.
            // If your TmdbMovieDetails has required properties, fill them in here.
            var responseJson = "{}";

            var handler = new CapturingRawJsonHandler(responseJson);
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) };

            var factory = Substitute.For<IHttpClientFactory>();
            factory.CreateClient("tmdb").Returns(httpClient);

            var options = Options.Create(new TmdbOptions { ApiKey = ApiKey });

            var service = new TmdbService(factory, options);

            // Act
            var result = await service.GetMovieDetailsAsync(movieId);

            // Assert
            handler.LastRequestUri.Should().NotBeNull();
            handler.LastRequestUri!.ToString().Should().Be(BaseUrl + expectedPathAndQuery);

            // We only assert it didn't throw + called correct URL.
            // If you want to assert properties, return a filled JSON object matching your model.
            result.Should().NotBeNull();
        }

        /// <summary>
        /// Captures the outgoing request and responds with JSON generated from an object.
        /// </summary>
        private sealed class CapturingJsonHandler : HttpMessageHandler
        {
            public Uri? LastRequestUri { get; private set; }

            private readonly string _json;

            public CapturingJsonHandler(object responseObject)
            {
                _json = JsonSerializer.Serialize(responseObject);
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequestUri = request.RequestUri;

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_json, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
        }

        /// <summary>
        /// Captures the outgoing request and responds with raw JSON (useful when your model is big).
        /// </summary>
        private sealed class CapturingRawJsonHandler : HttpMessageHandler
        {
            public Uri? LastRequestUri { get; private set; }
            private readonly string _json;

            public CapturingRawJsonHandler(string json)
            {
                _json = json;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequestUri = request.RequestUri;

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_json, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
        }
    }
}

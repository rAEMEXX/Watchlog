using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Implementations;
using Watchlog.Models.Domain.Entities;
using Xunit;

namespace Watchlog.UnitTests
{
    public class UserProgressServiceTests
    {
        private readonly IRepository<UserTitle> _userTitles;
        private readonly IRepository<UserTitleProgress> _progress;
        private readonly IRepository<Season> _seasons;
        private readonly IRepository<Title> _titles;
        private readonly UserProgressService _service;

        public UserProgressServiceTests()
        {
            _userTitles = Substitute.For<IRepository<UserTitle>>();
            _progress = Substitute.For<IRepository<UserTitleProgress>>();
            _seasons = Substitute.For<IRepository<Season>>();
            _titles = Substitute.For<IRepository<Title>>();

            _service = new UserProgressService(_userTitles, _progress, _seasons, _titles);
        }

        [Fact]
        public async Task UpdateProgressAsync_Throws_WhenUserIdMissing()
        {
            Func<Task> act = async () => await _service.UpdateProgressAsync("", 1, 1, 1);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateProgressAsync_ThrowsUnauthorized_WhenTitleNotOwned()
        {
            _userTitles.Query().Returns(new List<UserTitle>().AsQueryable().BuildMock());

            Func<Task> act = async () => await _service.UpdateProgressAsync("u1", 10, 1, 1);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task UpdateProgressAsync_CreatesProgress_ClampsEpisode_AndSetsWatching()
        {
            var userId = "u1";
            var titleId = 10;

            _userTitles.Query().Returns(new List<UserTitle>
            {
                new UserTitle { UserId = userId, TitleId = titleId }
            }.AsQueryable().BuildMock());

            // season has only 5 eps, request 999 -> clamp to 5
            _seasons.Query().Returns(new List<Season>
            {
                new Season { TitleId = titleId, SeasonNumber = 1, EpisodeCount = 5 },
                new Season { TitleId = titleId, SeasonNumber = 2, EpisodeCount = 10 }, // last season
            }.AsQueryable().BuildMock());

            _progress.Query().Returns(new List<UserTitleProgress>().AsQueryable().BuildMock());

            _titles.Query().Returns(new List<Title>
            {
                new Title { Id = titleId, Name = "Test Show" }
            }.AsQueryable().BuildMock());

            var msg = await _service.UpdateProgressAsync(userId, titleId, season: 1, episode: 999);

            await _progress.Received(1).AddAsync(Arg.Any<UserTitleProgress>(), Arg.Any<CancellationToken>());
            await _progress.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

            msg.Should().Contain("Progress updated");
            msg.Should().Contain("Season 1");
            msg.Should().Contain("Episode 5");
        }

        [Fact]
        public async Task UpdateProgressAsync_SetsCompleted_WhenLastEpisodeOfLastSeason()
        {
            var userId = "u1";
            var titleId = 10;

            _userTitles.Query().Returns(new List<UserTitle>
            {
                new UserTitle { UserId = userId, TitleId = titleId }
            }.AsQueryable().BuildMock());

            _seasons.Query().Returns(new List<Season>
            {
                new Season { TitleId = titleId, SeasonNumber = 1, EpisodeCount = 5 },
                new Season { TitleId = titleId, SeasonNumber = 2, EpisodeCount = 10 }, // last season
            }.AsQueryable().BuildMock());

            _progress.Query().Returns(new List<UserTitleProgress>
            {
                new UserTitleProgress { UserId = userId, TitleId = titleId }
            }.AsQueryable().BuildMock());

            _titles.Query().Returns(new List<Title>
            {
                new Title { Id = titleId, Name = "Test Show" }
            }.AsQueryable().BuildMock());

            var msg = await _service.UpdateProgressAsync(userId, titleId, season: 2, episode: 10);

            msg.Should().Contain("completed");
            await _progress.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task MarkWatchedAsync_CreatesProgress_AndSetsCompleted()
        {
            var userId = "u1";
            var titleId = 10;

            _userTitles.Query().Returns(new List<UserTitle>
            {
                new UserTitle { UserId = userId, TitleId = titleId }
            }.AsQueryable().BuildMock());

            _progress.Query().Returns(new List<UserTitleProgress>().AsQueryable().BuildMock());

            _titles.Query().Returns(new List<Title>
            {
                new Title { Id = titleId, Name = "Test Show" }
            }.AsQueryable().BuildMock());

            var msg = await _service.MarkWatchedAsync(userId, titleId);

            await _progress.Received(1).AddAsync(Arg.Any<UserTitleProgress>(), Arg.Any<CancellationToken>());
            await _progress.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            msg.Should().Contain("completed");
        }
    }
}
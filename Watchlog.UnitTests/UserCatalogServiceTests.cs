using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Implementations;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.ViewModels;
using Xunit;

namespace Watchlog.UnitTests
{
    public class UserCatalogServiceTests
    {
        private readonly IRepository<UserTitle> _userTitles;
        private readonly IRepository<UserTitleProgress> _progress;
        private readonly IMapper _mapper;
        private readonly UserCatalogService _service;

        public UserCatalogServiceTests()
        {
            _userTitles = Substitute.For<IRepository<UserTitle>>();
            _progress = Substitute.For<IRepository<UserTitleProgress>>();
            _mapper = Substitute.For<IMapper>();

            _service = new UserCatalogService(_userTitles, _progress, _mapper);
        }

        [Fact]
        public async Task GetCatalogAsync_ReturnsMappedItems_AndAppliesProgress_AndSortsSeasons()
        {
            var userId = "u1";

            var title = new Title
            {
                Id = 10,
                Name = "Show",
                Seasons = new List<Season>
                {
                    new Season { SeasonNumber = 2, EpisodeCount = 10 },
                    new Season { SeasonNumber = 1, EpisodeCount = 8 },
                }
            };

            _userTitles.Query().Returns(new List<UserTitle>
            {
                new UserTitle { UserId = userId, TitleId = 10, Title = title, AddedAt = DateTime.UtcNow }
            }.AsQueryable().BuildMock());

            _progress.Query().Returns(new List<UserTitleProgress>
            {
                new UserTitleProgress
                {
                    UserId = userId,
                    TitleId = 10,
                    CurrentSeason = 1,
                    CurrentEpisode = 3,
                    Status = "Watching"
                }
            }.AsQueryable().BuildMock());

            // mapper returns VM with seasons (unsorted), service sorts them
            _mapper.Map<TitleCatalogItemViewModel>(Arg.Any<Title>())
                .Returns(new TitleCatalogItemViewModel
                {
                    Id = 10,
                    Name = "Show",
                    Seasons = new List<SeasonOption>
                    {
                        new SeasonOption { SeasonNumber = 2 },
                        new SeasonOption { SeasonNumber = 1 }
                    }
                });

            var result = await _service.GetCatalogAsync(userId);

            result.Should().HaveCount(1);
            result[0].CurrentSeason.Should().Be(1);
            result[0].CurrentEpisode.Should().Be(3);
            result[0].Status.Should().Be("Watching");

            result[0].Seasons.Select(s => s.SeasonNumber).Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task RemoveFromCatalogAsync_DeletesLink_WhenExists()
        {
            var userId = "u1";
            var titleId = 10;

            var link = new UserTitle { Id = 1, UserId = userId, TitleId = titleId };

            _userTitles.Query().Returns(new List<UserTitle> { link }.AsQueryable().BuildMock());

            await _service.RemoveFromCatalogAsync(userId, titleId);

            _userTitles.Received(1).Delete(Arg.Is<UserTitle>(x => x == link));
            await _userTitles.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RemoveFromCatalogAsync_DoesNothing_WhenMissing()
        {
            _userTitles.Query().Returns(new List<UserTitle>().AsQueryable().BuildMock());

            await _service.RemoveFromCatalogAsync("u1", 10);

            _userTitles.DidNotReceive().Delete(Arg.Any<UserTitle>());
            await _userTitles.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
using Fan.Blog.Data;
using Fan.Blog.Helpers;
using Fan.Blog.Models;
using Fan.Blog.Services;
using Fan.Blog.Services.Interfaces;
using Fan.Exceptions;
using Fan.Settings;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fan.Blog.Tests.Services
{
    public class CategoryServiceTest
    {
        private readonly ICategoryService categoryService;
        private readonly Mock<ICategoryRepository> catRepoMock = new Mock<ICategoryRepository>();
        private readonly Mock<IMediator> mediatorMock = new Mock<IMediator>();
        private readonly IDistributedCache cache;

        public CategoryServiceTest()
        {
            // cache, logger
            var serviceProvider = new ServiceCollection().AddMemoryCache().AddLogging().BuildServiceProvider();
            cache = new MemoryDistributedCache(serviceProvider.GetService<IOptions<MemoryDistributedCacheOptions>>());
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<CategoryService>();

            // settings
            var settingSvcMock = new Mock<ISettingService>();
            settingSvcMock.Setup(svc => svc.GetSettingsAsync<CoreSettings>()).Returns(Task.FromResult(new CoreSettings()));
            settingSvcMock.Setup(svc => svc.GetSettingsAsync<BlogSettings>()).Returns(Task.FromResult(new BlogSettings()));

            // setup the default category in db
            var defaultCat = new Category { Id = 1, Title = "Web Development", Slug = "web-development" };
            catRepoMock.Setup(c => c.GetAsync(1)).Returns(Task.FromResult(defaultCat));
            catRepoMock.Setup(r => r.GetListAsync()).Returns(Task.FromResult(new List<Category> { defaultCat }));

            categoryService = new CategoryService(catRepoMock.Object, settingSvcMock.Object, mediatorMock.Object, cache, logger);
        }

        [Fact]
        public async void TestCreateAsync_ShouldReturnException_GivenNull()
        {
            string title = null;
            await Assert.ThrowsAsync<FanException>(() => categoryService.CreateAsync(title));
        }

        [Fact]
        public async void TestCreateAsync_ShouldReturnException_GivenEmptyTitle()
        {
            string title = "";
            await Assert.ThrowsAsync<FanException>(() => categoryService.CreateAsync(title));
        }

        [Fact]
        public async void TestCreateAsync_ShouldReturnFanException_WhenTitleAlreadyExists()
        {
            var title = "web development";
            await Assert.ThrowsAsync<FanException>(() => categoryService.CreateAsync(title));
        }

        [Fact]
        public async void TestDeleteAsync_ShouldReturnFanException_WhenTryingToDeleteDefaultCategory()
        {
            await Assert.ThrowsAsync<FanException>(() => categoryService.DeleteAsync(1));
        }

        [Fact]
        public async void Update_category_calls_repo_and_invalidates_cache_for_all_categories()
        {
            // Arrange
            var cat = await categoryService.GetAsync(1);

            // Act
            cat.Title = "Cat1";
            await categoryService.UpdateAsync(cat);

            // Assert
            catRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Exactly(1));
            Assert.Null(await cache.GetAsync(BlogCache.KEY_ALL_CATS));
        }

        [Fact]
        public async void TestUpdateAsync_ShouldReturnFanException_WhenGivenNull()
        {
            await Assert.ThrowsAsync<FanException>(() => categoryService.UpdateAsync(null));
        }

        [Fact]
        public async void TestUpdateAsync_ShouldReturnFanException_WhenGivenEmptyCategory()
        {
            await Assert.ThrowsAsync<FanException>(() => categoryService.UpdateAsync(new Category()));
        }

        [Fact]
        public async void TestUpdateAsync_ShouldReturnFanException_WhenGivenCategoryWithoutTitle()
        {
            await Assert.ThrowsAsync<FanException>(() => categoryService.UpdateAsync(new Category { Id = 1 }));
        }

        [Fact]
        public async void Update_category_with_title_changed_only_in_casing_is_OK()
        {
            var cat = await categoryService.GetAsync(1);

            cat.Title = "web development";

            var catAgain = await categoryService.UpdateAsync(cat);
            Assert.Equal(1, catAgain.Id);
            Assert.Equal("web development", catAgain.Title);
        }
    }
}

using Fan.Blog.Data;
using Fan.Blog.Enums;
using Fan.Blog.Events;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fan.Blog.Tests.Services
{
    public class BlogPostServiceTest
    {
        private readonly CancellationToken cancellationToken = new CancellationToken();
        private readonly Mock<IMediator> mediatorMock = new Mock<IMediator>();
        private readonly Mock<IPostRepository> postRepoMock = new Mock<IPostRepository>();
        private readonly BlogPostService blogPostService;

        public BlogPostServiceTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddMemoryCache()
                .AddLogging()
                .BuildServiceProvider();

            var cache = new MemoryDistributedCache(serviceProvider.GetService<IOptions<MemoryDistributedCacheOptions>>());

            var logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<BlogPostService>();

            var mapper = BlogUtil.Mapper;
            var settingServiceMock = new Mock<ISettingService>();

            settingServiceMock
                .Setup(svc => svc.GetSettingsAsync<CoreSettings>())
                .Returns(Task.FromResult(new CoreSettings()));

            settingServiceMock
                .Setup(svc => svc.GetSettingsAsync<BlogSettings>())
                .Returns(Task.FromResult(new BlogSettings()));

            var imageServiceMock = new Mock<IImageService>();

            blogPostService = new BlogPostService(
                settingServiceMock.Object,
                imageServiceMock.Object,
                postRepoMock.Object,
                cache,
                logger,
                mapper,
                mediatorMock.Object
            );
        }

        [Theory]
        [InlineData("Blog post title", "blog-post-title", "blog-post-title-2")]
        [InlineData("Blog post title 2", "blog-post-title-2", "blog-post-title-3")]
        public async void TestCreatePost_shouldAlwaysProduceUniqueSlug(string title, string slug, string expectedSlug)
        {
            var postId = 1;
            var date = DateTimeOffset.Now;

            postRepoMock
                .Setup(r => r.GetAsync(slug, date.Year, date.Month, date.Day))
                .Returns(Task.FromResult(new Post { Id = 1, Slug = slug }));

            var actualSlug = await blogPostService.GetBlogPostSlugAsync(title, date, ECreateOrUpdate.Create, postId);

            Assert.Equal(expectedSlug, actualSlug);
        }

        [Theory]
        [InlineData(null, EPostStatus.Published, "'Title' must not be empty.")]
        [InlineData("", EPostStatus.Published, "'Title' must not be empty.")]
        public async void TestPublishBlogPost_shouldThrowExceptionWhenTitleIsEmptyOrNull(string title, EPostStatus status, string expectedMessage)
        {
            var blogPost = new BlogPost { Title = title, Status = status };

            var exception = await Assert.ThrowsAsync<FanException>(() => blogPost.ValidateTitleAsync());

            Assert.Equal(expectedMessage, exception.ValidationErrors[0].ErrorMessage);
        }

        [Theory]
        [InlineData(EPostStatus.Draft, "The length of 'Title' must be 250 characters or fewer. You entered 251 characters.")]
        [InlineData(EPostStatus.Published, "The length of 'Title' must be 250 characters or fewer. You entered 251 characters.")]
        public async void TestValidateTitle_blogPostTitleCannotBeMoreThan250Chars(EPostStatus status, string expectedMessage)
        {
            var title = string.Join("", Enumerable.Repeat<char>('x', 251));
            var blogPost = new BlogPost { Title = title, Status = status };

            var exception = await Assert.ThrowsAsync<FanException>(() => blogPost.ValidateTitleAsync());

            Assert.Equal(expectedMessage, exception.ValidationErrors[0].ErrorMessage);
        }

        [Fact]
        public async void TestCreateAsync_shouldThrowArgumentNullException_givenParameterIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => blogPostService.CreateAsync(null));
        }

        [Fact]
        public async void TestUpdateAsync_shouldThrowArgumentException_givenParameterIsNull()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => blogPostService.UpdateAsync(null));
        }
    }
}
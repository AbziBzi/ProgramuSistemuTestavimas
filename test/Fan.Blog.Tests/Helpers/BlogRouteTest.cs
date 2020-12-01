using Fan.Blog.Helpers;
using System;
using Xunit;

namespace Fan.Blog.Tests.Helpers
{
    public class BlogRoutesTest
    {
        [Fact]
        public void TestGetPostRelativeLink_ShouldReturnFoundLink_GivenRightCreationDateAndSlug()
        {
            var createdOn = new DateTimeOffset(2018, 9, 9, 0, 0, 0, TimeSpan.Zero);
            var slug = "my-post";
            var relativeLink = BlogRoutes.GetPostRelativeLink(createdOn, slug);

            Assert.StartsWith("/", relativeLink);
            Assert.Equal("/post/2018/09/09/my-post", relativeLink);
        }

        [Fact]
        public void TestGetPostPreviewRelativeLink_ShouldReturnFoundPreviewLink_Test_GivenRightCreationDateAndSlug()
        {
            var createdOn = new DateTimeOffset(2018, 9, 9, 0, 0, 0, TimeSpan.Zero);
            var slug = "my-post";
            var relativeLink = BlogRoutes.GetPostPreviewRelativeLink(createdOn, slug);

            Assert.StartsWith("/", relativeLink);
            Assert.Equal("/preview/post/2018/09/09/my-post", relativeLink);
        }

        [Theory]
        [InlineData(1, "/blog/post/1")]
        [InlineData(10, "/blog/post/10")]
        [InlineData(111, "/blog/post/111")]
        [InlineData(2222, "/blog/post/2222")]
        [InlineData(55555, "/blog/post/55555")]
        [InlineData(-10, "/blog/post/-10")]
        public void TestGetPostPermalink_ShouldReturnFoundPermaLink_GivenPostId(int postId, string expectedPermaLink)
        {
            var permalink = BlogRoutes.GetPostPermalink(postId);

            Assert.Equal(expectedPermaLink, permalink);
        }

        [Theory]
        [InlineData(1, "/admin/compose/post/1")]
        [InlineData(10, "/admin/compose/post/10")]
        [InlineData(111, "/admin/compose/post/111")]
        [InlineData(2222, "/admin/compose/post/2222")]
        [InlineData(55555, "/admin/compose/post/55555")]
        [InlineData(-10, "/admin/compose/post/-10")]
        public void TestPostEditLink_ShouldReturnEditLink_GivenPostId(int postId, string expectedEditLink)
        {
            var editLink = BlogRoutes.GetPostEditLink(postId);

            Assert.Equal(expectedEditLink, editLink);
        }

        [Theory]
        [InlineData("technology", "/blog/technology")]
        [InlineData("a", "/blog/a")]
        [InlineData("152", "/blog/152")]
        [InlineData("?=asdd", "/blog/?=asdd")]
        [InlineData("As-aff?=516aA", "/blog/As-aff?=516aA")]
        public void TestGetCategoryRelativeLink_ShouldReturnRelativeLink_GivenSlug(string slug,
            string expectedRelativeLink)
        {
            var relativeLink = BlogRoutes.GetCategoryRelativeLink(slug);

            Assert.Equal(expectedRelativeLink, relativeLink);
        }

        [Fact]
        public void TestGetCategoryRssRelativeLink_ShouldReturnRssLink_GivenSlug()
        {
            var slug = "technology";
            var rssLink = BlogRoutes.GetCategoryRssRelativeLink(slug);

            Assert.Equal("/blog/technology/feed", rssLink);
        }

        [Fact]
        public void TestGetTagRelativeLink_ShouldReturnRelativeLink_GivenSlug()
        {
            var slug = "asp-net-core";
            var relativeLink = BlogRoutes.GetTagRelativeLink(slug);

            Assert.Equal("/posts/tagged/asp-net-core", relativeLink);
        }

        [Fact]
        public void TestGetArchiveRelativeLink_ShouldReturnRelativeLink_GivenYearAndMonth()
        {
            var year = 2018;
            var month = 9;
            var relativeLink = BlogRoutes.GetArchiveRelativeLink(year, month);

            Assert.StartsWith("/", relativeLink);
            Assert.Equal("/posts/2018/09", relativeLink);
        }
    }
}
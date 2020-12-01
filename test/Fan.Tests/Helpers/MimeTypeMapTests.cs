using System;
using System.Collections.Generic;
using System.Text;
using Fan.Helpers;
using Xunit;

namespace Fan.Tests.Helpers
{
    public class MimeTypeMapTest
    {
        [Theory]
        [InlineData(".323", "text/h323")]
        [InlineData(".cod", "image/cis-cod")]
        [InlineData(".mp4v", "video/mp4")]
        [InlineData(".apk", "application/vnd.android.package-archive")]
        [InlineData(".avi", "video/x-msvideo")]
        [InlineData(".chm", "application/octet-stream")]
        [InlineData(".cs", "text/plain")]
        public void GetMimeType_MimeType(string extension, string expectedMimeType)
        {
            string actualMimeType = MimeTypeMap.GetMimeType(extension);
            Assert.Equal(expectedMimeType, actualMimeType);
        }

        [Theory]
        [InlineData("adba")]
        [InlineData("ddas")]
        [InlineData("fasfasf")]
        [InlineData("a")]
        [InlineData("123")]
        [InlineData(".ABC.")]
        [InlineData(".A?bda4519?-=")]
        public void GetMimeType_DefaultMime_GivenBadExtension(string extension)
        {
            string actualMimeType = MimeTypeMap.GetMimeType(extension);
            Assert.Equal("application/octet-stream", actualMimeType);
        }

        [Fact]
        public void GetMimeType_ArgumentException_GivenNullExtension()
        {
            Assert.Throws<ArgumentNullException>(() => MimeTypeMap.GetMimeType(null));
        }

        [Theory]
        [InlineData("text/h323", ".323")]
        [InlineData("image/cis-cod", ".cod")]
        [InlineData("video/mp4", ".mp4")]
        [InlineData("text/plain", ".txt")]
        [InlineData("application/octet-stream", ".bin")]
        public void GetExtension_FoundExtension(string mimeType, string expectedExtension)
        {
            string actualExtension = MimeTypeMap.GetExtension(mimeType);
            Assert.Equal(expectedExtension, actualExtension);
        }

        [Theory]
        [InlineData("asd")]
        [InlineData("1235")]
        [InlineData("fasfa13213")]
        [InlineData("a")]
        [InlineData("A")]
        [InlineData("Ava.?=")]
        [InlineData("15265")]
        [InlineData("Ad=A8462_(7")]
        public void GetExtension_ArgumentException_GivenBadMimeType(string mimeType)
        {
            Assert.Throws<ArgumentException>(() => MimeTypeMap.GetExtension(mimeType));
        }
    }
}
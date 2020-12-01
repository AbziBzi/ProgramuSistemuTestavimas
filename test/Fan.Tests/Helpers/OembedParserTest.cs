using Fan.Helpers;
using Xunit;

namespace Fan.Tests.Helpers
{
    public class OembedParserTest
    {
        [Theory]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A")]
        [InlineData("https://youtu.be/M6T6CSiJv-A")]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A&w=800&h=400&start=75")]
        [InlineData("https://youtu.be/M6T6CSiJv-A?t=254")]
        public void TestGetOembedType_ReturnsTypeYouTube_GivenYoutubeUrl(string url)
        {
            Assert.Equal(EEmbedType.YouTube, OembedParser.GetOembedType(url));
        }

        [Fact]
        public void TestGetOembedType_ReturnsTypeVimeo_GivenVimeoUrl()
        {
            Assert.Equal(EEmbedType.Vimeo, OembedParser.GetOembedType("https://vimeo.com/1084537"));
        }


        [Theory]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A&w=800&h=400&start=75", "M6T6CSiJv-A&w=800&h=400&start=75")]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A&t=762s", "M6T6CSiJv-A&start=762s")]
        [InlineData("https://www.youtube.com/embed/M6T6CSiJv-A&amp;t=726s", "M6T6CSiJv-A&amp;start=726s")]
        [InlineData("https://youtu.be/M6T6CSiJv-A", "M6T6CSiJv-A")]
        [InlineData("https://youtu.be/M6T6CSiJv-A?t=254", "M6T6CSiJv-A?t=254")]

        public void TestGetYouTubeVideoKey_ReturnKey_GivenYoutubeUrl(string url, string expected)
        {
            Assert.Equal(expected, OembedParser.GetYouTubeVideoKey(url));
        }

        [Theory]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A", "M6T6CSiJv-A")]
        [InlineData("https://www.youtube.com/embed/M6T6CSiJv-A", "M6T6CSiJv-A")]
        [InlineData("https://youtu.be/M6T6CSiJv-A", "M6T6CSiJv-A")]
        [InlineData("https://youtu.be/M6T6CSiJv-A?t=254", "M6T6CSiJv-A?t=254")]
        
        public void TestGetYouTubeEmbed_ReturnsYoutubeEmbedHtml_GivenYoutubeUrl(string url, string id)
        {
            string expected = @"<iframe width=""800"" height=""450"" src =""https://www.youtube.com/embed/"+id+@""" frameborder =""0"" allow=""autoplay; encrypted - media"" allowfullscreen></iframe>";
            Assert.Equal(expected, OembedParser.GetYouTubeEmbed(url));
        }

        [Theory]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A&w=800&h=400&start=75", 800, 400, 75)]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A&w=560&h=315&start=0", 560, 315, 0)]
        [InlineData("https://www.youtube.com/watch?v=M6T6CSiJv-A&w=1920&h=1080&start=900", 1920, 1080, 900)]
        public void TestGetYouTubeEmbed_ReturnsYoutubeEmbedWithInfo_GivenYoutubeUrl(string url, int width, int height, int start)
        {
            string expected;
            if (start == 0)
            {
                expected = $@"<iframe width=""{width}"" height=""{height}"" src =""https://www.youtube.com/embed/M6T6CSiJv-A"" frameborder =""0"" allow=""autoplay; encrypted - media"" allowfullscreen></iframe>";
            }
            else expected = $@"<iframe width=""{width}"" height=""{height}"" src =""https://www.youtube.com/embed/M6T6CSiJv-A?start={start}"" frameborder =""0"" allow=""autoplay; encrypted - media"" allowfullscreen></iframe>";
            Assert.Equal(expected, OembedParser.GetYouTubeEmbed(url));
        }

        [Theory]
        [InlineData("https://vimeo.com/451993692", 451993692)]
        [InlineData("https://vimeo.com/106947229", 106947229)]
        [InlineData("https://vimeo.com/-106947229", -106947229)]

        public void TestGetVimeoEmbed_ReturnVimeoEmbedHtml_GivenVimeoUrl(string url, int id)
        {
            string expected = @"<iframe width=""800"" height=""450"" src =""https://player.vimeo.com/video/"+id+@""" frameborder =""0"" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>";
            Assert.Equal(expected, OembedParser.GetVimeoEmbed(url));
        }

        [Theory]
        [InlineData(@"<figure class=""media""><oembed url=""https://www.youtube.com/watch?v=M6T6CSiJv-A""></oembed></figure>",
            @"<figure class=""media""><iframe width=""800"" height=""450"" src=""https://www.youtube.com/embed/M6T6CSiJv-A"" frameborder=""0"" allow=""autoplay; encrypted - media"" allowfullscreen=""""></iframe></figure>")]
        [InlineData(@"<figure class=""media""><oembed url=""https://vimeo.com/451993692""></oembed></figure>",
            @"<figure class=""media""><iframe width=""800"" height=""450"" src=""https://player.vimeo.com/video/451993692"" frameborder=""0"" webkitallowfullscreen="""" mozallowfullscreen="""" allowfullscreen=""""></iframe></figure>")]
        public void TestParse_ReturnHtml_GivenHtmlEmbed(string body, string expected)
        {
            Assert.Equal(expected, OembedParser.Parse(body));
        }
    }
}

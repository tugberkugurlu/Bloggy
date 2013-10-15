using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bloggy.Client.Web.Tests
{
    public class SanitizeHTMLExtensionFacts
    {
        [Fact]
        public void ToSanitizedHtml_should_allow_p_element()
        {
            string html = "Lorem Ipsum. <p>Lorem</p> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_acronym_element()
        {
            string html = "Lorem Ipsum. <acronym>Lorem</acronym> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_abbr_element()
        {
            string html = "Lorem Ipsum. <abbr>Lorem</abbr> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_img_element()
        {
            //actually, this shouldn't be allowed but no problem for now
            string html = "Lorem Ipsum. <img>Lorem</img> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_img_element_with_src_attribute()
        {
            //actually, this shouldn't be allowed but no problem for now
            string html = "Lorem Ipsum. <img src=\"http://example.com/foo.jpg\">Lorem</img> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_img_element_with_src_and_alt_attribute()
        {
            string html = "Lorem Ipsum. <img alt=\"foo bar\" src=\"http://example.com/foo.jpg\">Lorem</img> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_img_element_with_src_and_width_attribute()
        {
            string html = "Lorem Ipsum. <img width=\"300\" src=\"http://example.com/foo.jpg\">Lorem</img> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_img_element_with_src_and_height_attribute()
        {
            string html = "Lorem Ipsum. <img height=\"300\" src=\"http://example.com/foo.jpg\">Lorem</img> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_blockquote_element()
        {
            string html = "Lorem Ipsum. <blockquote>Lorem</blockquote> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_blockquote_element_with_cite_attribute()
        {
            string html = "Lorem Ipsum. <blockquote cite=\"foo bar\">Lorem</blockquote> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_br_element()
        {
            string html = "Lorem Ipsum. <br/> Lorem. <br/> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_strong_element()
        {
            string html = "Lorem Ipsum. <strong>Lorem.</strong> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_em_element()
        {
            string html = "Lorem Ipsum. <em>Lorem.</em> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_u_element()
        {
            string html = "Lorem Ipsum. <u>Lorem.</u> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_strike_element()
        {
            string html = "Lorem Ipsum. <strike>Lorem.</strike> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_a_element_and_append_rel_nofollow()
        {
            // Arrange
            string html = "Lorem Ipsum. <a>Lorem.</a> Lorem Ipsum.";
            string expected = "Lorem Ipsum. <a rel=\"nofollow\">Lorem.</a> Lorem Ipsum.";

            // Act
            string output = html.ToSanitizedHtml();

            // Assert
            Assert.Equal<string>(expected, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_not_allow_a_element_with_rel_attribute_and_append_rel_nofollow()
        {
            // Arrange
            string html = "Lorem Ipsum. <a rel=\"foo\">Lorem.</a> Lorem Ipsum.";
            string expected = "Lorem Ipsum. <a rel=\"nofollow\">Lorem.</a> Lorem Ipsum.";

            // Act
            string output = html.ToSanitizedHtml();

            // Assert
            Assert.Equal<string>(expected, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_a_element_with_href_and_append_rel_nofollow()
        {
            // Arrange
            string html = "Lorem Ipsum. <a href=\"http://example.com\">Lorem.</a> Lorem Ipsum.";
            string expected = "Lorem Ipsum. <a href=\"http://example.com\" rel=\"nofollow\">Lorem.</a> Lorem Ipsum.";

            // Act
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(expected, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_ol_element()
        {
            //actually, this shouldn't be allowed but no problem for now
            string html = "Lorem Ipsum. <ol>Lorem.</ol> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_ol_element_with_li_element()
        {
            string html = "Lorem Ipsum. <ol><li>Lorem</li><li>Ipsum</li></ol> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_ul_element()
        {
            //actually, this shouldn't be allowed but no problem for now
            string html = "Lorem Ipsum. <ul>Lorem.</ul> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_allow_ul_element_with_li_element()
        {
            string html = "Lorem Ipsum. <ul><li>Lorem</li><li>Ipsum</li></ul> Lorem Ipsum.";
            string output = html.ToSanitizedHtml();

            Assert.Equal<string>(html, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_not_allow_script_element()
        {
            string html = "Lorem Ipsum. <script>alert('Boom!');</script> Lorem Ipsum.";
            string expected = "Lorem Ipsum. alert('Boom!'); Lorem Ipsum.";

            // Act
            var output = html.ToSanitizedHtml();

            // Assert
            Assert.Equal<string>(expected, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_not_allow_div_element()
        {
            string html = "Lorem Ipsum. <div>foo.</div> Lorem Ipsum.";
            string expected = "Lorem Ipsum. foo. Lorem Ipsum.";

            // Act
            string output = html.ToSanitizedHtml();

            // Assert
            Assert.Equal<string>(expected, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_not_allow_a_element_with_any_of_its_attributes_except_for_href_and_append_rel_nofollow()
        {
            string html = "Lorem Ipsum. <a href=\"http://example.com\" title=\"foo bar\">Lorem.</a> Lorem Ipsum.";
            string expected = "Lorem Ipsum. <a href=\"http://example.com\" rel=\"nofollow\">Lorem.</a> Lorem Ipsum.";

            // Act
            string output = html.ToSanitizedHtml();

            // Assert
            Assert.Equal<string>(expected, output);
        }

        [Fact]
        public void ToSanitizedHtml_should_not_allow_script_element_but_should_allow_a_and_append_rel_nofollow()
        {
            // Arrange
            string html = "Lorem Ipsum. <script>alert('Boom!');</script> Lorem Ipsum. <a href=\"http://example.com\">example.com</a>";
            string expected = "Lorem Ipsum. alert('Boom!'); Lorem Ipsum. <a href=\"http://example.com\" rel=\"nofollow\">example.com</a>";

            // Act
            string output = html.ToSanitizedHtml();

            // Assert
            Assert.Equal<string>(expected, output);
        }
    }
}

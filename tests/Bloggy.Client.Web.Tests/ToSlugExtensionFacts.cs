using System;
using Xunit;

namespace Bloggy.Client.Web.Tests
{
    public class ToSlugExtensionFacts
    {
        [Fact]
        public void Should_Return_Expected_Slug()
        {
            // Arrange
            string input = "Replace the Default Server of OwinHost.exe with Nowin in Visual Studio 2013";
            string expected = "replace-the-default-server-of-owinhost-exe-with-nowin-in-visual-studio-2013";

            // Act
            string output = input.ToSlug();

            // Assert
            Assert.Equal(expected, output, StringComparer.Ordinal);
        }

        [Fact]
        public void Should_Return_Expected_Slug_For_C_Sharp()
        {
            // Arrange
            string input = "Replace for C#";
            string expected = "replace-for-c-sharp";

            // Act
            string output = input.ToSlug();

            // Assert
            Assert.Equal(expected, output, StringComparer.Ordinal);
        }
    }
}

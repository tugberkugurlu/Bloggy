﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bloggy.Client.Web
{
    /// <remarks>
    /// Thanks to Nick Berardi's awesome Sanitize HTML utility. Refer to http://refactormycode.com/codes/333-sanitize-html.
    /// </remarks>
    public static class StringExtensions
    {
        private static readonly Regex HtmlTagExpression = new Regex(@"(?'tag_start'</?)(?'tag'\w+)((\s+(?'attr'(?'attr_name'\w+)(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+)))?)+\s*|\s*)(?'tag_end'/?>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex WhiteSpaceBetweenHtmlTagsExpression = new Regex(@">(/w+)<", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex HtmlLineBreakExpression = new Regex(@"<br(/s+)/>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Dictionary<string, List<string>> ValidHtmlTags = new Dictionary<string, List<string>> {
            { "p", new List<string>() },
            { "br", new List<string>() }, 
            { "strong", new List<string>() }, 
            { "em", new List<string>() }, 
            { "u", new List<string>() }, 
            { "strike", new List<string>() }, 
            { "ol", new List<string>() }, 
            { "ul", new List<string>() }, 
            { "li", new List<string>() }, 
            { "a", new List<string> { "href" } }, 
            { "img", new List<string> { "src", "height", "width", "alt" } },
            { "blockquote", new List<string> { "cite" } }, 
            { "abbr", new List<string>() }, 
            { "acronym", new List<string>() }, 
        };

        /// <summary>
        /// Toes the safe HTML.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ToSanitizedHtml(this string text)
        {
            return text.RemoveInvalidHtmlTags();
        }

        /// <summary>
        /// Removes the invalid HTML tags.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string RemoveInvalidHtmlTags(this string text)
        {
            return HtmlTagExpression.Replace(text, new MatchEvaluator((Match m) =>
            {
                if (!ValidHtmlTags.ContainsKey(m.Groups["tag"].Value))
                {
                    return string.Empty;
                }

                StringBuilder generatedTag = new StringBuilder(m.Length);

                System.Text.RegularExpressions.Group tagStart = m.Groups["tag_start"];
                System.Text.RegularExpressions.Group tagEnd = m.Groups["tag_end"];
                System.Text.RegularExpressions.Group tag = m.Groups["tag"];
                System.Text.RegularExpressions.Group tagAttributes = m.Groups["attr"];

                generatedTag.Append(tagStart.Success ? tagStart.Value : "<");
                generatedTag.Append(tag.Value);

                foreach (Capture attr in tagAttributes.Captures)
                {
                    int indexOfEquals = attr.Value.IndexOf('=');

                    // don't proceed any futurer if there is no equal sign or just an equal sign
                    if (indexOfEquals < 1)
                    {
                        continue;
                    }

                    string attrName = attr.Value.Substring(0, indexOfEquals);

                    // check to see if the attribute name is allowed and write attribute if it is
                    if (ValidHtmlTags[tag.Value].Contains(attrName))
                    {
                        generatedTag.Append(' ');
                        generatedTag.Append(attr.Value);
                    }
                }

                // add nofollow to all hyperlinks
                if (tagStart.Success && tagStart.Value == "<" && tag.Value.Equals("a", StringComparison.OrdinalIgnoreCase))
                {
                    generatedTag.Append(" rel=\"nofollow\"");
                }

                generatedTag.Append(tagEnd.Success ? tagEnd.Value : ">");

                return generatedTag.ToString();
            }));
        }
    }
}
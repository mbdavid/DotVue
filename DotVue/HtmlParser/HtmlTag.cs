using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotVue
{
    /// <summary>
    /// A very simple root tag parser. Parse HTML content and returns a list of tags with attributes/content
    /// </summary>
    internal class HtmlTag
    {
        /// <summary>
        /// Get tag name
        /// </summary>
        public string TagName { get; private set; }

        /// <summary>
        /// Get all attributes in this tag
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Get inner HTML content (exclude tag itself)
        /// </summary>
        public StringBuilder InnerHtml { get; set; } = new StringBuilder();

        public HtmlTag(string tagName)
        {
            this.TagName = tagName;
        }
    }
}
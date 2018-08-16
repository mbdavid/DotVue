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
    internal class Tag
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

        private Tag()
        {
        }

        /// <summary>
        /// Get outer HTML content (include tag itself)
        /// </summary>
        public string OuterHtml
        {
            get
            {
                return "<" + this.TagName + 
                    string.Join("", this.Attributes.Select(x => " " + x.Key + "=\"" + x.Value.Replace("\"", "&quot;") + "\"")) +
                    ">" + 
                    this.InnerHtml + 
                    "</" + this.TagName + ">";
            }
        }

        /// <summary>
        /// Get attribute value - return default if not exists
        /// </summary>
        public string GetAttribute(string name, string defaultValue = null)
        {
            if (this.Attributes.TryGetValue(name, out string value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Parse HTML content and return a list of tags
        /// </summary>
        public static List<Tag> ParseHtml(string html)
        {
            var tags = new List<Tag>();
            var s = new StringScanner(html.Trim(), RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            while (!s.HasTerminated)
            {
                var tag = ReadTag(s);

                if (tag != null)
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        private static Tag ReadTag(StringScanner s)
        {
            // discard non tag text before
            if (!s.Match(@"[\s\S]*?<\w+")) return null;

            var tag = new Tag { TagName = s.Scan(@"[\s\S]*?<(\w+)", 1).ToLower() };

            // read attributes
            while (!s.HasTerminated)
            {
                var key = s.Scan(@"\s*([\w:@\-\.]+)\s*=?\s*", 1);
                var aspa = s.Scan(@"[""']");
                var value = aspa.Length == 0 ?
                    s.Scan(@"\w+") :
                    s.Scan(@"([\s\S]*?)" + aspa, 1);

                if (key.Length > 0) tag.Attributes[key] = value;

                if (s.Read(1) == ">") break;
            }

            while (!s.HasTerminated)
            {
                // read comments
                if (s.Match(@"<!--"))
                {
                    tag.InnerHtml.Append(s.Scan(@"<!--[\s\S]*?-->"));
                }
                // read possible stack
                if (s.Match(@"<" + tag.TagName))
                {
                    tag.InnerHtml.Append(ReadTag(s).OuterHtml);
                }
                // read closing tag
                if (s.Scan(@"<\/" + tag.TagName + @">").Length > 0)
                {
                    break;
                }

                // adding content
                tag.InnerHtml.Append(s.Read(1));
            }

            return tag;
        }
    }
}
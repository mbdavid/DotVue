using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LiteDB
{
    /// <summary>
    /// A very simple root tag parser
    /// </summary>
    public class Tag
    {
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public StringBuilder InnerHtml { get; set; } = new StringBuilder();

        public string OuterHtml
        {
            get
            {
                return "<" + this.Name + 
                    string.Join("", this.Attributes.Select(x => " " + x.Key + "=\"" + x.Value.Replace("\"", "&quot;") + "\"")) +
                    ">" + 
                    this.InnerHtml + 
                    "</" + this.Name + ">";
            }
        }

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

            var tag = new Tag { Name = s.Scan(@"[\s\S]*?<(\w+)", 1) };

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
                if (s.Match(@"<" + tag.Name))
                {
                    tag.InnerHtml.Append(ReadTag(s).OuterHtml);
                }
                // read closing tag
                if (s.Scan(@"<\/" + tag.Name + @">").Length > 0)
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotVue
{
    /// <summary>
    /// Represent a single file with root tag sections and directives
    /// </summary>
    internal class HtmlFile
    {
        /// <summary>
        /// Directive @name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Directive @viewmodel
        /// </summary>
        public string ViewModel { get; private set; }

        /// <summary>
        /// Directive @auth
        /// </summary>
        public bool Auth { get; private set; }

        /// <summary>
        /// Directive @role
        /// </summary>
        public List<string> Roles { get; private set; } = new List<string>();

        /// <summary>
        /// Tag [template]
        /// </summary>
        public string Template { get; private set; }

        /// <summary>
        /// Tags [style]
        /// </summary>
        public List<string> Styles { get; private set; } = new List<string>();

        /// <summary>
        /// Tags [script]...[/script]
        /// </summary>
        public List<string> ClientScripts { get; private set; } = new List<string>();

        /// <summary>
        /// Tags [script src="...."][/script] (string = src) or [link href="..." ref="stylesheet"]
        /// </summary>
        public List<string> Includes { get; private set; } = new List<string>();

        public HtmlFile(Stream stream)
        {
            var content = "";

            using (stream)
            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            var s = new StringScanner(content, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            while (!s.HasTerminated)
            {
                var directive = this.ReadDirective(s);

                if (directive.Key == null) break;

                switch(directive.Key)
                {
                    case "name":
                        this.Name = directive.Value;
                        break;
                    case "viewmodel":
                        this.ViewModel = directive.Value;
                        break;
                    case "auth":
                        this.Auth = true;
                        break;
                    case "role":
                        this.Roles.Add(directive.Value);
                        break;
                }
            }

            while(!s.HasTerminated)
            {
                var tag = this.ReadTag(s);

                if (tag == null) break;

                switch(tag.TagName)
                {
                    case "template":
                        this.Template = tag.InnerHtml.ToString().Trim();
                        break;
                    case "style":
                        this.Styles.Add(tag.InnerHtml.ToString().Trim());
                        break;
                    case "link":
                        if (tag.Attributes.TryGetValue("href", out var href))
                        {
                            this.Includes.Add(href);
                        }
                        break;
                    case "script":
                        if(tag.Attributes.TryGetValue("src", out var src))
                        {
                            this.Includes.Add(src);
                        }
                        else
                        {
                            this.ClientScripts.Add(tag.InnerHtml.ToString().Trim());
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Read @directive from header file
        /// </summary>
        private KeyValuePair<string, string> ReadDirective(StringScanner s)
        {
            if (!s.Match(@"\s*@\w+")) return new KeyValuePair<string, string>(null, null);

            var key = s.Scan(@"\s*@(\w+)\s*", 1).ToLower();
            var value = s.Scan(@".*").Trim();

            return new KeyValuePair<string, string>(key, value);
        }

        /// <summary>
        /// Read root tags from file
        /// </summary>
        private HtmlTag ReadTag(StringScanner s)
        {
            // discard non tag text before
            if (!s.Match(@"[\s\S]*?<[\w-]+")) return null;

            var tag = new HtmlTag(s.Scan(@"[\s\S]*?<([\w-]+)", 1).ToLower());

            // read attributes
            while (!s.HasTerminated)
            {
                var key = s.Scan(@"\s*([\w:@\-\.]+)\s*=?\s*", 1);
                var quote = s.Scan(@"[""']");
                var value = quote.Length == 0 ?
                    s.Scan(@"\w+") :
                    s.Scan(@"([\s\S]*?)" + quote, 1);

                if (key.Length > 0) tag.Attributes[key] = value;

                if (s.Scan(@"\/>").Length > 0)
                {
                    return tag; // self closed tag
                }

                if (s.Read(1) == ">") break;
            }

            if (tag.TagName.Equals("link", StringComparison.OrdinalIgnoreCase)) return tag;

            var expr = (tag.TagName.Equals("template", StringComparison.OrdinalIgnoreCase)) ? 
                @"([\s\S]*)</" + tag.TagName + ">" : 
                @"([\s\S]*?)</" + tag.TagName + ">";

            tag.InnerHtml.Append(s.Scan(expr, 1));

            return tag;
        }
    }
}
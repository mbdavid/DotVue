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
        /// Load all @directive in header file
        /// </summary>
        public Dictionary<string, string> Directives { get; }

        /// <summary>
        /// Tag [template]
        /// </summary>
        public StringBuilder Template { get; } = new StringBuilder();

        /// <summary>
        /// Tags [style] and [style scoped]
        /// </summary>
        public StringBuilder Styles { get; } = new StringBuilder();

        /// <summary>
        /// Tags [script]...[/script]
        /// </summary>
        public StringBuilder Scripts { get; } = new StringBuilder();

        /// <summary>
        /// Tags [script mixin]...[/script]
        /// </summary>
        public List<string> Mixins { get; } = new List<string>();

        /// <summary>
        /// Get directive value (if exists). Returns null if not exists
        /// </summary>
        public string GetDirective(string key)
        {
            if (this.Directives.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        public HtmlFile(string content, StringBuilder globalScripts)
        {
            var s = new StringScanner(content, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            this.Directives = this.ReadDirectives(s).ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

            var scoped = false;
            var scopedId = "s-" + Guid.NewGuid().ToString("d").Substring(0, 6);

            while (!s.HasTerminated)
            {
                if (s.Match(@"<style\s+scoped>([\s\S]*?)</style>"))
                {
                    if (scoped)
                    {
                        this.Template.Append("<h1>Only 1 style scoped supported<h1>");
                    }
                    else
                    {
                        scoped = true;

                        var style = s.Scan(@"<style\s+scoped>([\s\S]*?)</style>", 1);

                        this.Styles.Append($"[{scopedId}] {{ {style} }}");
                    }
                }
                else if (s.Match(@"<style>([\s\S]*?)</style>"))
                {
                    this.Styles.Append(s.Scan(@"<style>([\s\S]*?)</style>", 1));
                }
                else if (s.Match(@"<script>([\s\S]*?)</script>"))
                {
                    this.Scripts.Append(s.Scan(@"<script>([\s\S]*?)</script>", 1));
                }
                else if (s.Match(@"<script\s+mixin>([\s\S]*?)</script>"))
                {
                    this.Mixins.Add(s.Scan(@"<script\s+mixin>([\s\S]*?)</script>", 1));
                }
                else if (s.Match(@"<script\s+global>([\s\S]*?)</script>"))
                {
                    globalScripts.Append(s.Scan(@"<script\s+global>([\s\S]*?)</script>", 1));
                }
                else if (s.Match(@"<template>([\s\S]*?)</template>"))
                {
                    this.Template.Append(s.Scan(@"<template>([\s\S]*?)</template>", 1));
                }
                else
                {
                    this.Template.Append(s.Read(1));
                }
            }

            // change template if scoped was used
            if (scoped)
            {
                var htm = Regex.Replace(this.Template.ToString(), @"^\s*(<[\w-]+)", $"$1 {scopedId}");

                this.Template.Clear();

                this.Template.Append(htm);
            }
        }

        /// <summary>
        /// Read @directive from header file
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> ReadDirectives(StringScanner s)
        {
            while(!s.HasTerminated)
            {
                var key = s.Scan(@"\s*@(\w+)", 1);

                if (key.Length == 0) yield break;

                var value = s.Scan(@"[\s\S]*?\n").Trim();

                yield return new KeyValuePair<string, string>(key, string.IsNullOrWhiteSpace(value) ? null : value);
            }
        }
    }
}
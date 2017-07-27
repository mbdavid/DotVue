using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Represent a compoenent definition that will generate Vue options
    /// </summary>
    internal class ComponentDefinition
    {
        private string _template;

        /// <summary>
        /// Returns text template removing %place_holders%
        /// </summary>
        public string Template { get { return Regex.Replace(_template, @"%\w+%", ""); } }
        public List<string> Scripts { get; private set; } = new List<string>();
        public List<string> Styles { get; private set; } = new List<string>();
        public List<Tag> Tags { get; set; }
        public JObject Data { get; private set; } = new JObject();
        public HashSet<string> Props { get; private set; } = new HashSet<string>();
        public Dictionary<string, Tuple<string, string, string[]>> Methods { get; private set; } = new Dictionary<string, Tuple<string, string, string[]>>();
        public Dictionary<string, string> Watch { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Computed { get; private set; } = new Dictionary<string, string>();
        public HashSet<string> Locals { get; private set; } = new HashSet<string>();
        public bool CreatedHook { get; private set; } = false;

        /// <summary>
        /// Extract from Type and Content all information to map a server ViewModel class into a Vue component
        /// </summary>
        public void ExtractMetadata(Type type, string content)
        {
            // populate Template/Scripts/Styles
            this.ParseContent(content);

            // instance vm to get data structure + computed
            using (var vm = (ViewModel)Activator.CreateInstance(type))
            {
                var viewModel = JObject.FromObject(vm, Config.JSettings);

                // merge viewModel into current data
                this.Data.Merge(viewModel);

                // get all computed
                foreach (var c in type
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.FieldType == typeof(Computed)))
                {
                    this.Computed[c.Name] = ((Computed)c.GetValue(vm)).Code;
                }
            }

            // add all props (defined by Prop attribute)
            foreach (var p in type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<PropAttribute>() != null))
            {
                // props MUST be read-only to avoid user change it (is passed by caller)
                //if(p.CanWrite) throw new NotSupportedException("Properties marked as [Prop] must be read-only");

                this.Props.Add(p.Name);
            }

            // only call Created method if created was override in component
            var created = type.GetMethod("OnCreated", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var oncreated = created.GetBaseDefinition().DeclaringType != created.DeclaringType;

            // get all public methods
            var methods = type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => !x.IsSpecialName)
                .ToList();

            // add to methods to render
            if (oncreated)
            {
                this.CreatedHook = true;
                methods.Add(created); // add created method to method list
            }

            // get all public methods
            foreach(var m in methods)
            {
                // checks if method contains Script attribute (will run before call $update)
                var scripts = m.GetCustomAttributes<ScriptAttribute>(true);
                var pre = string.Join("", scripts.Where(x => !string.IsNullOrWhiteSpace(x.Pre)).Select(x => "\n      " + x.Pre));
                var post = string.Join("", scripts.Where(x => !string.IsNullOrWhiteSpace(x.Post)).Select(x => "\n            " + x.Post));
                var parameters = m.GetParameters().Select(x => x.Name).ToArray();

                this.Methods[m.Name] = new Tuple<string, string, string[]>(pre, post, parameters);
            }

            // get all watch variables (are methods finish as _Watch or marked with Watch attribute)
            foreach (var w in type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.Name.EndsWith("_Watch", StringComparison.InvariantCultureIgnoreCase) || x.GetCustomAttribute<WatchAttribute>() != null))
            {
                var name = w.GetCustomAttribute<WatchAttribute>()?.Name ?? w.Name.Substring(0, w.Name.LastIndexOf("_"));

                this.Watch[name] = w.Name;
            }

            // get all local variables (that will not send from client to server)
            foreach(var l in type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<LocalAttribute>() != null)
                .Select(x => x.Name))
            {
                this.Locals.Add(l);
            }
        }

        #region ParseContent

        private void ParseContent(string html)
        {
            this.Tags = Tag.ParseHtml(html);

            foreach(var tag in this.Tags)
            {
                var lang = tag.GetAttribute("lang") ?? tag.GetAttribute("language");
                var content = Config.Instance.RunCompiler(lang, tag.InnerHtml.ToString());
                var slot = tag.GetAttribute("for");

                if (tag.Name == "style")
                {
                    this.Styles.Add(content);
                }
                else if (tag.Name == "script")
                {
                    this.Scripts.Add(content);
                }
                else if(tag.Name == "template")
                {
                    _template = slot == null ?
                        content :
                        Regex.Replace(_template, @"%" + slot + "%", m =>
                        {
                            return content + "%" + slot + "%";
                        });
                }
            }
        }

        #endregion
        
    }
}

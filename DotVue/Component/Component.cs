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
    public class Component
    {
        private JsonSerializerSettings _serializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = VueContractResolver.Instance
        };

        /// <summary>
        /// Component virtual path
        /// </summary>
        public string VPath { get; set; }

        /// <summary>
        /// ViewModel type class
        /// </summary>
        public Type ViewModelType { get; set; }

        /// <summary>
        /// List of css style rules to be loaded when component are loaded
        /// </summary>
        public List<string> Styles { get; set; } = new List<string>();

        /// <summary>
        /// Mixin functions to be merged into main vue options
        /// </summary>
        public List<string> Scripts { get; set; } = new List<string>();

        /// <summary>
        /// Component template
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// List of component properties (props)
        /// </summary>
        public List<string> Props { get; set; } = new List<string>();

        /// <summary>
        /// Initial data options
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// List of all methods that will be run as server
        /// </summary>
        public List<MethodInfo> Methods { get; set; }

        /// <summary>
        /// List of all watch methods
        /// </summary>
        public List<string> Watch { get; set; } = new List<string>();

        /// <summary>
        /// List of all fields that are used as computed
        /// </summary>
        public List<FieldInfo> Computed { get; set; } = new List<FieldInfo>();

        /// <summary>
        /// Indicate to use "created" function to server call
        /// </summary>
        public bool CreatedHook { get; set; }

        public List<Plugin> Plugins { get; set; } = new List<Plugin>();

        public Component(string vpath, Type viewModelType, string content, List<Plugin> plugins)
        {
            this.VPath = vpath;
            this.ViewModelType = viewModelType;
            this.ParseContent(content);
            this.Plugins = plugins;

            this.ExtractMetatadata(viewModelType);

            foreach(var p in plugins)
            {
                this.ExtractMetatadata(p.GetType());
            }
        }

        #region RenderScript

        public void RenderScript(TextWriter writer)
        {
            using (var vm = (ViewModel)Activator.CreateInstance(this.ViewModelType))
            {
                RenderScript(vm, writer);
            }
        }

        private void RenderScript(ViewModel vm, TextWriter writer)
        {
            writer.Write("//\n");
            writer.WriteFormat("// Component: \"{0}\"\n", VPath);
            writer.Write("//\n");

            // add "style" before return Vue object
            if (Styles.Count > 0)
            {
                writer.WriteFormat("Vue.$addStyle('{0}');\n",
                    string.Join("", Styles).EncodeJavascript());
            }

            writer.Write("return {\n");

            if (this.Props.Count > 0)
            {
                writer.WriteFormat("  props: [{0}],\n", string.Join(", ", this.Props.Select(x => "'" + x + "'")));
            }

            // only call Created method if created was override in component
            if (this.CreatedHook)
            {
                writer.Write("  created: function() {\n");
                writer.Write("    this.OnCreated();\n");
                writer.Write("  },\n");
            }

            // append template string
            writer.WriteFormat("  template: '{0}',\n", this.Template.EncodeJavascript());
            writer.WriteFormat("  data: function() {{\n    return {0};\n  }},\n", JsonConvert.SerializeObject(vm, _serializeSettings));

            if (this.Methods.Count > 0)
            {
                writer.Write("  methods: {\n");

                foreach (var m in this.Methods)
                {
                    // checks if method contains Script attribute (will run before call $update)
                    var scripts = m.GetCustomAttributes<ScriptAttribute>(true);
                    var pre = string.Join("", scripts.Where(x => !string.IsNullOrWhiteSpace(x.Pre)).Select(x => "\n      " + x.Pre));
                    var post = string.Join("", scripts.Where(x => !string.IsNullOrWhiteSpace(x.Post)).Select(x => "\n            " + x.Post));

                    // get all parameters without HttpPostFile parameters
                    var parameters = m.GetParameters()
                        .Select(x => x.Name);

                    writer.WriteFormat("    {0}: function({1}) {{{2}\n      this.$update(this, '{0}', [{3}]){4};\n    }}{5}\n",
                        m.Name,
                        string.Join(", ", m.GetParameters().Select(x => x.Name)),
                        pre,
                        string.Join(", ", parameters),
                        post.Length > 0 ? "\n          .then(function(vm) { (function() {" + post + "\n          }).call(vm); })" : "",
                        m == this.Methods.Last() ? "" : ",");
                }

                writer.Write("  },\n");
            }

            if (this.Computed.Count > 0)
            {
                writer.Write("  computed: {\n");

                foreach (var c in this.Computed)
                {
                    writer.WriteFormat("    {0}: function() {{\n      return ({1})(this);\n    }}{2}\n",
                        c.Name,
                        ((Computed)c.GetValue(vm)).Code,
                        c == this.Computed.Last() ? "" : ",");
                }

                writer.Write("  },\n");
            }

            if (this.Watchs.Count > 0)
            {
                writer.Write("  watch: {\n");

                foreach (var w in this.Watch)
                {
                    var name = w.GetCustomAttribute<WatchAttribute>()?.Name ?? w.Name.Substring(0, w.Name.LastIndexOf("_"));

                    writer.WriteFormat("    {0}: {{\n      handler: function(v, o) {{\n        if (this.$updating) return false;\n        this.{1}(v, o);\n      }},\n      deep: true\n    }}{2}\n",
                        name, 
                        w.Name,
                        w == this.Watchs.Last() ? "" : ",");
                }

                writer.Write("  },\n");
            }

            if (this.Scripts.Count > 0)
            {
                writer.WriteFormat("  mixins: [\n");

                foreach(var s in this.Scripts)
                {
                    writer.WriteFormat("           (function() {{\n{0}\n  }})() || {{}}{1}\n",
                        s,
                        s == this.Scripts.Last() ? "" : ",");
                }

                writer.WriteFormat("  ],");
            }

            writer.WriteFormat("  vpath: '{0}'\n", this.VPath);
            writer.Write("}");
        }

        #endregion

        #region Template Parser

        private static Dictionary<string, Func<string, string>> _compilers = new Dictionary<string, Func<string, string>>(StringComparer.InvariantCultureIgnoreCase);

        private static Regex _reTemplate = new Regex(@"<template(\s+lang(uage)?=[""'](?<lang>.*)[""']\s*)?>\s*(?<content>[\s\S]*)\s*<\/template>");
        private static Regex _reStyle = new Regex(@"<style(\s+lang(uage)?=[""'](?<lang>.*)[""']\s*)?>\s*(?<content>[\s\S]*?)\s*<\/style>");
        private static Regex _reScript = new Regex(@"<script(\s+lang(uage)?=[""'](?<lang>.*)[""']\s*)?>\s*(?<content>[\s\S]*?)\s*<\/script>");

        /// <summary>
        /// Register new compiler for an tag (template|style|script|[custom]), like: Component.RegisterCompiler('style', 'less', s => LessCompiler.Compile(s));
        /// </summary>
        public static void RegisterCompiler(string tag, string lang, Func<string, string> compiler)
        {
            if (string.IsNullOrEmpty(lang)) throw new ArgumentNullException(nameof(lang));
            if (compiler == null) throw new ArgumentNullException(nameof(compiler));

            _compilers[tag + "/" + lang] = compiler;
        }

        private void ParseContent(string content)
        {
            var template = _reTemplate.Match(content);
            var style = _reStyle.Match(content);
            var script = _reScript.Match(content);

            this.Template = RunCompiler("template", template.Groups["lang"].Value, template.Groups["content"].Value);
            var css = RunCompiler("style", style.Groups["lang"].Value, style.Groups["content"].Value);
            var code = RunCompiler("script", script.Groups["lang"].Value, script.Groups["content"].Value);

            if (!string.IsNullOrEmpty(css)) this.Styles.Add(css);
            if (!string.IsNullOrEmpty(code)) this.Scripts.Add(code);
        }

        private string RunCompiler(string tag, string lang, string content)
        {
            if (string.IsNullOrEmpty(lang)) return content;

            Func<string, string> compiler;

            if (_compilers.TryGetValue(tag + "/" + lang, out compiler))
            {
                return compiler(content);
            }

            throw new ArgumentException("Tag " + tag.ToString().ToLower() + " contains a not defined language attribute (" + lang + "). Use Component.RegisterCompiler");
        }

        #endregion

        /// <summary>
        /// Extract from a class metadata to Vue options (methods, watch, data, computed) and add to class variables
        /// </summary>
        private void ExtractMetatadata(Type type)
        {
            // add all props
            this.Props.AddRange(type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<PropAttribute>() != null)
                .Select(x => x.Name));

            // only call Created method if created was override in component
            var created = type.GetMethod("OnCreated", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var oncreated = created.GetBaseDefinition().DeclaringType != created.DeclaringType;

            // add to methods to render
            if (oncreated)
            {
                this.CreatedHook = true;
                this.Methods.Add(created);
            }

            // get all public methods
            this.Methods.AddRange(type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => !x.IsSpecialName));

            // get all computed
            this.Computed.AddRange(type
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Computed)));

            // get all watch variables (finish as _Watch or marked with Watch attribute)
            this.Watch.AddRange(type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.Name.EndsWith("_Watch", StringComparison.InvariantCultureIgnoreCase) || x.GetCustomAttribute<WatchAttribute>() != null)
                .Select(x => x.Name));
        }

        #region Update Model

        public void UpdateModel(string data, string props, string method, JToken[] parameters, HttpFileCollection files, TextWriter writer)
        {
            using (var vm = (ViewModel)Activator.CreateInstance(ViewModelType))
            {
                var o = new JObject();

                

                JsonConvert.PopulateObject(data, vm, _serializeSettings);
                JsonConvert.PopulateObject(props, vm, _serializeSettings);

                if (!string.IsNullOrEmpty(method))
                {
                    ExecuteMethod(vm, method, parameters, files);
                }

                RenderUpdate(vm, data, writer);
            }
        }

        private void ExecuteMethod(ViewModel vm, string name, JToken[] parameters, HttpFileCollection files)
        {
            var methods = ViewModelType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.Name == name)
                .Where(x => x.IsFamily || x.IsPublic)
                .ToList();

            if (methods.Count == 0 || methods.Count > 1) throw new SystemException("Method " + name + " do not exists, are not public/protected or has more than one signature");

            var method = methods.First();
            var pars = new List<object>();
            var index = 0;

            foreach (var p in method.GetParameters())
            {
                var token = parameters[index++];

                if (p.ParameterType == typeof(HttpPostedFile))
                {
                    var value = ((JValue)token).Value.ToString();

                    pars.Add(files.GetMultiple(value).FirstOrDefault());
                }
                else if (p.ParameterType == typeof(List<HttpPostedFile>) || p.ParameterType == typeof(IList<HttpPostedFile>))
                {
                    var value = ((JValue)token).Value.ToString();

                    pars.Add(new List<HttpPostedFile>(files.GetMultiple(value)));
                }
                else if (token.Type == JTokenType.Object)
                {
                    var obj = ((JObject)token).ToObject(p.ParameterType);

                    pars.Add(obj);
                }
                else if (token.Type == JTokenType.String && p.ParameterType.IsEnum)
                {
                    var value = ((JValue)token).Value.ToString();

                    pars.Add(Enum.Parse(p.ParameterType, value));
                }
                else
                {
                    var value = ((JValue)token).Value;

                    pars.Add(Convert.ChangeType(value, p.ParameterType));
                }
            }

            ViewModel.Execute(vm, method, pars.ToArray());
        }

        private void RenderUpdate(ViewModel vm, string model, TextWriter writer)
        {
            var original = JObject.Parse(model);
            var current = JObject.FromObject(vm, new JsonSerializer { ContractResolver = VueContractResolver.Instance });
            var diff = new JObject();

            foreach (var item in current)
            {
                var o = original[item.Key];

                if (original[item.Key] == null && item.Value.HasValues == false) continue;

                if (!JToken.DeepEquals(original[item.Key], item.Value))
                {
                    diff[item.Key] = item.Value;
                }
            }

            var output = new JObject
            {
                { "update", diff },
                { "script", ViewModel.Script(vm) }
            };

            using (var w = new JsonTextWriter(writer))
            {
                output.WriteTo(w);
            }
        }

        #endregion
    }
}

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotVue
{
    /// <summary>
    /// </summary>
    internal class ComponentLoader
    {
        private readonly IServiceProvider _service;
        private readonly Dictionary<string, Func<HtmlTag, string>> _compilers;

        public ComponentLoader(IServiceProvider service, Dictionary<string, Func<HtmlTag, string>> compilers)
        {
            _service = service;
            _compilers = compilers;
        }

        public ComponentInfo Load(ComponentDiscover discover)
        {
            var type = discover.File.ViewModel == null ?
                typeof(ViewModel) :
                discover.Assembly.GetType(discover.File.ViewModel, true);

            var component = new ComponentInfo
            {
                Name = discover.Name,
                Template = this.Compile(discover.File.Template),
                InheritAttrs = discover.File.InheritAttrs ?? true,
                Styles = discover.File.Styles.Select(x => this.Compile(x)).ToList(),
                Scripts = discover.File.ClientScripts.Select(x => this.Compile(x)).ToList(),
                Includes = discover.File.Includes,
                IsAutenticated = discover.File.IsAutenticated,
                Roles = discover.File.Roles.ToArray(),
                ViewModelType = type,
                Data = this.GetData(type),
                Props = this.GetProps(type).ToList(),
                Locals = this.GetLocals(type).ToList(),
                Computed = this.GetComputed(type).ToDictionary(x => x.Key, x => x.Value),
                Watch = this.GetWatch(type).ToDictionary(x => x.Key, x => x.Value),
                Methods = this.GetMethods(type).ToDictionary(x => x.Method.Name, x => x)
            };

            return component;
        }

        /// <summary>
        /// Get component name based on resource name
        /// </summary>
        public static string GetName(string fullname)
        {
            var arr = fullname.Split('.');

            return arr[arr.Length - 2];
        }

        /// <summary>
        /// Compile html/css/js based on lang="" attribute
        /// </summary>
        private string Compile(HtmlTag tag)
        {
            if (tag.Attributes.TryGetValue("lang", out var lang) && _compilers.TryGetValue(lang, out var func))
            {
                return func(tag);
            }
            else
            {
                return tag.InnerHtml.ToString();
            }
        }

        /// <summary>
        /// Get default ViewModel data object
        /// </summary>
        private JObject GetData(Type type)
        {
            using (var vm = (ViewModel)ActivatorUtilities.CreateInstance(_service, type))
            {
                return JObject.FromObject(vm, JsonSettings.JsonSerializer);
            }
        }

        /// <summary>
        /// Get all properties defined as [Prop] attribute
        /// </summary>
        private IEnumerable<string> GetProps(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<PropAttribute>() != null)
                .Select(x => x.Name);
        }

        /// <summary>
        /// Get all properties defined as [Local] attribute
        /// </summary>
        private IEnumerable<string> GetLocals(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<LocalAttribute>() != null)
                .Select(x => x.Name);
        }

        /// <summary>
        /// Get all properties defined as [Computed] attribute (ComputedName, JsCode)
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> GetComputed(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => new KeyValuePair<string, string>(x.Name, x.GetCustomAttribute<ComputedAttribute>(true)?.Code))
                .Where(x => x.Value != null);
        }

        /// <summary>
        /// Get all properties defined as [Computed] attribute (PropertyWatched, MethodName)
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> GetWatch(Type type)
        {
            return type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.Name.EndsWith("_Watch", StringComparison.InvariantCultureIgnoreCase) || x.GetCustomAttribute<WatchAttribute>() != null)
                .Select(x => new KeyValuePair<string, string>(x.GetCustomAttribute<WatchAttribute>()?.Name ?? x.Name.Substring(0, x.Name.LastIndexOf("_")), x.Name));
        }

        /// <summary>
        /// Get all methods
        /// </summary>
        private IEnumerable<ViewModelMethod> GetMethods(Type type)
        {
            // only call Created method if created was override in component
            var created = type.GetMethod("OnCreated", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var oncreated = created.GetBaseDefinition().DeclaringType != created.DeclaringType;

            if (oncreated)
            {
                yield return this.GetViewModelMethod(created);
            }

            var methods = type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => !x.IsSpecialName)
                .Where(x => x.Name != "Dispose");

            foreach(var m in methods)
            {
                yield return this.GetViewModelMethod(m);
            }
        }

        /// <summary>
        /// Create view model method based on MethodInfo
        /// </summary>
        private ViewModelMethod GetViewModelMethod(MethodInfo m)
        {
            var scripts = m.GetCustomAttributes<ScriptAttribute>(true).ToArray();
            var autorize = m.GetCustomAttribute<AutorizeAttribute>(true);

            var pre = scripts.Where(x => !string.IsNullOrWhiteSpace(x.Pre)).Select(x => x.Pre).ToArray();
            var post = scripts.Where(x => !string.IsNullOrWhiteSpace(x.Post)).Select(x => x.Post).ToArray();
            var parameters = m.GetParameters().Select(x => x.Name).ToArray();
            var roles = autorize?.Roles ?? new string[0];

            return new ViewModelMethod
            {
                Method = m,
                Pre = pre,
                Post = post,
                Parameters = parameters,
                IsAuthenticated = autorize != null,
                Roles = roles
            };
        }
    }
}

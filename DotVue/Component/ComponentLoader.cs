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

        public ComponentLoader(IServiceProvider service)
        {
            _service = service;
        }

        public ComponentInfo Load(string fullname, Stream stream, Assembly assembly)
        {
            HtmlFile html;

            using (var r = new StreamReader(stream))
            {
                var content = r.ReadToEnd();
                html = new HtmlFile(content);
            }

            var type = this.GetViewModelType(html, assembly);

            var component = new ComponentInfo
            {
                Name = html.Name ?? GetName(fullname),
                Template = html.Template,
                Styles = html.Styles,
                Scripts = html.ClientScripts,
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
        private string GetName(string fullname)
        {
            var arr = fullname.Split('.');

            return arr[arr.Length - 2];
        }

        /// <summary>
        /// Get viewModel type from directive or inline code (or empty view model)
        /// </summary>
        private Type GetViewModelType(HtmlFile html, Assembly assembly)
        {
            if (html.ViewModel != null)
            {
                // get viewmode based on @viewmodel directive
                return assembly.GetType(html.ViewModel, true);
            }
            else
            {
                // otherwise return empty viewmodel
                return typeof(ViewModel);
            }
        }

        /// <summary>
        /// Get default ViewModel data object
        /// </summary>
        public JObject GetData(Type type)
        {
            using (var vm = (ViewModel)ActivatorUtilities.CreateInstance(_service, type))
            {
                return JObject.FromObject(vm, JsonSettings.JsonSerializer);
            }
        }

        /// <summary>
        /// Get all properties defined as [Prop] attribute
        /// </summary>
        public IEnumerable<string> GetProps(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<PropAttribute>() != null)
                .Select(x => x.Name);
        }

        /// <summary>
        /// Get all properties defined as [Local] attribute
        /// </summary>
        public IEnumerable<string> GetLocals(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<LocalAttribute>() != null)
                .Select(x => x.Name);
        }

        /// <summary>
        /// Get all properties defined as [Computed] attribute (ComputedName, JsCode)
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetComputed(Type type)
        {
            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => new KeyValuePair<string, string>(x.Name, x.GetCustomAttribute<ComputedAttribute>(true)?.Code))
                .Where(x => x.Value != null);
        }

        /// <summary>
        /// Get all properties defined as [Computed] attribute (PropertyWatched, MethodName)
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetWatch(Type type)
        {
            return type
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.Name.EndsWith("_Watch", StringComparison.InvariantCultureIgnoreCase) || x.GetCustomAttribute<WatchAttribute>() != null)
                .Select(x => new KeyValuePair<string, string>(x.GetCustomAttribute<WatchAttribute>()?.Name ?? x.Name.Substring(0, x.Name.LastIndexOf("_")), x.Name));
        }

        /// <summary>
        /// Get all methods
        /// </summary>
        public IEnumerable<ViewModelMethod> GetMethods(Type type)
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
            var roles = m.GetCustomAttribute<AutorizeAttribute>(true);

            var pre = scripts.Where(x => !string.IsNullOrWhiteSpace(x.Pre)).Select(x => x.Pre).ToArray();
            var post = scripts.Where(x => !string.IsNullOrWhiteSpace(x.Post)).Select(x => x.Post).ToArray();
            var parameters = m.GetParameters().Select(x => x.Name).ToArray();

            return new ViewModelMethod
            {
                Method = m,
                Pre = pre,
                Post = post,
                Parameters = parameters
            };
        }
    }
}

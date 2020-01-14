using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotVue
{
    /// <summary>
    /// </summary>
    internal class ComponentLoader
    {
        private readonly IServiceProvider _service;
        private readonly string _root;

        public ComponentLoader(IServiceProvider service)
        {
            _service = service;

            var env = service.GetService<IHostingEnvironment>();

            _root = env.ContentRootPath;
        }

        public ComponentInfo Load(Assembly assembly, string resourceName)
        {
            // try read file from local disk (for hot reload)
            var localFile = Path.Combine(_root + @"\..\",
                assembly.GetName().Name +
                    resourceName.Substring(assembly.GetName().Name.Length, resourceName.Length - assembly.GetName().Name.Length - 4).Replace(".", "\\")) +
                ".vue";

            var content = File.Exists(localFile) ?
                File.ReadAllText(localFile) :
                new StreamReader(assembly.GetManifestResourceStream(resourceName)).ReadToEnd();

            var html = new HtmlFile(content);
            var name = html.GetDirective("name") ?? Path.GetFileNameWithoutExtension(localFile);
            var component = new ComponentInfo(name);

            if (html.Directives.TryGetValue("viewmodel", out var viewModel))
            {
                component.ViewModelType = assembly.GetType(viewModel, true);
            }
            else
            {
                // try guess viewModel based on resource name
                component.ViewModelType = assembly.GetType(resourceName.Substring(0, resourceName.Length - 4)) ?? typeof(ViewModel);
            }

            component.IsAsync = html.Directives.ContainsKey("async");

            if (html.Directives.TryGetValue("page", out var route))
            {
                component.IsPage = true;
                component.Route = route;
            }

            component.Template = html.Template.ToString().Trim();
            component.Styles = html.Styles.ToString();
            component.Scripts = html.Scripts.ToString() + this.GetScriptAttr(component.ViewModelType);
            component.Mixins = html.Mixins;

            using (var instance = (ViewModel)ActivatorUtilities.CreateInstance(_service, component.ViewModelType))
            {
                component.JsonData = this.GetJsonData(instance);
                component.Methods = this.GetMethods(component.ViewModelType).ToDictionary(x => x.Method.Name, x => x, StringComparer.OrdinalIgnoreCase);

                component.Props = this.GetField<PropAttribute>(component.ViewModelType, instance);
                component.RouteParams = this.GetField<RouteParamAttribute>(component.ViewModelType, instance);
                component.QueryString = this.GetField<QueryStringAttribute>(component.ViewModelType, instance);

                component.Cookies = component.ViewModelType
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<CookieAttribute>() != null)
                    .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
            }

            component.InheritAttrs = !component.Template.Contains("v-bind=\"$attrs\"");

            component.IsAuthenticated = component.ViewModelType.GetCustomAttribute<AutorizeAttribute>() != null;

            if (component.IsAuthenticated)
            {
                component.Roles = component.ViewModelType.GetCustomAttribute<AutorizeAttribute>().Roles;
            }

            return component;
        }

        /// <summary>
        /// Get default ViewModel as json data
        /// </summary>
        private JObject GetJsonData(ViewModel instance)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ContractResolver = new CustomContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            return JObject.Parse(JsonConvert.SerializeObject(instance, jsonSettings));
        }

        /// <summary>
        /// Get all field defined as [Prop, RouteParam, QueryString] attribute
        /// </summary>
        private Dictionary<string, object> GetField<T>(Type type, ViewModel instance)
            where T : Attribute
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<T>() != null)
                .ToDictionary(x => x.Name, x => x.GetValue(instance), StringComparer.OrdinalIgnoreCase);
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

            var parameters = m.GetParameters().Select(x => x.Name).ToArray();
            var roles = autorize?.Roles ?? new string[0];
            var watch = m.Name.EndsWith("_Watch", StringComparison.InvariantCultureIgnoreCase) || m.GetCustomAttribute<WatchAttribute>() != null ?
                m.GetCustomAttribute<WatchAttribute>()?.Name ?? m.Name.Substring(0, m.Name.LastIndexOf("_")) :
                null;

            return new ViewModelMethod
            {
                Method = m,
                Parameters = parameters,
                Watch = watch,
                IsAuthenticated = autorize != null,
                Roles = roles
            };
        }

        /// <summary>
        /// Get all scripts attribute on this class
        /// </summary>
        private string GetScriptAttr(Type type)
        {
            var script = new StringBuilder();

            foreach (var attr in type.GetCustomAttributes<ScriptAttribute>(true))
            {
                script.AppendLine(attr.Code.Replace("{name}", type.Name.ToCamelCase()));
            }

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.GetCustomAttributes<ScriptAttribute>(true).Count() > 0))
            {
                foreach (var attr in member.GetCustomAttributes<ScriptAttribute>(true))
                {
                    script.AppendLine(attr.Code.Replace("{name}", member.Name.ToCamelCase()));
                }
            }

            return script.ToString();
        }
    }
}

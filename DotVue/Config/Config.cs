using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace DotVue
{
    public class Config
    {
        private readonly Dictionary<string, Func<HtmlTag, string>> _compilers = new Dictionary<string, Func<HtmlTag, string>>();

        private readonly Dictionary<string, ComponentDiscover> _discovers = new Dictionary<string, ComponentDiscover>();

        // should be concurrent dictionary
        private readonly Dictionary<string, ComponentInfo> _components = new Dictionary<string, ComponentInfo>();

        public Config()
        {
        }

        /// <summary>
        /// Get/Set extension used in Vue components. Default: ".vue"
        /// </summary>
        public string Extension { get; set; } = ".vue";

        public JsonSerializer JsonSerializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = CustomContractResolver.Instance
        };

        public JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = CustomContractResolver.Instance
        };

        /// <summary>
        /// Add new assembly into vue handler components
        /// </summary>
        public void AddAssembly(Assembly assembly)
        {
            foreach (var resourceName in assembly
                .GetManifestResourceNames()
                .Where(x => Path.GetExtension(x) == Extension))
            {
                var file = new HtmlFile(assembly.GetManifestResourceStream(resourceName));
                var name = file.Name ?? ComponentLoader.GetName(resourceName);

                _discovers[name] = new ComponentDiscover
                {
                    Name = name,
                    Assembly = assembly,
                    File = file
                };
            }
        }

        /// <summary>
        /// Add string compiler to html/js/css used when tag contains lang="..."
        /// </summary>
        public void AddCompiler(string lang, Func<HtmlTag, string> func)
        {
            _compilers[lang] = func;
        }

        /// <summary>
        /// Add new assembly into vue handler components
        /// </summary>
        private void AddWebFiles(string root, Assembly assembly)
        {
            foreach (var path in Directory.GetFiles(root, "*" + Extension, SearchOption.AllDirectories))
            {
                var file = new HtmlFile(File.OpenRead(path));
                var name = file.Name ?? ComponentLoader.GetName(path.Replace(@"\", "."));

                _discovers[name] = new ComponentDiscover
                {
                    Name = name,
                    Assembly = assembly,
                    File = file
                };

                // remove component to be re-loaded on next Load method (only for debug)
                _components.Remove(name);
            }
        }

        /// <summary>
        /// Return all component that user has access
        /// </summary>
        internal IEnumerable<string> Discover(IPrincipal user, IServiceProvider service)
        {
            var env = service.GetService<IHostingEnvironment>();

            // in development, read physical root directory and find pages (not only embedded resources)
            if (env.IsDevelopment())
            {
                this.AddWebFiles(env.ContentRootPath, Assembly.GetEntryAssembly());
            }

            foreach (var c in _discovers.Values)
            {
                if (c.File.IsAutenticated && user.Identity.IsAuthenticated == false) continue;
                if (c.File.Roles.Count > 0 && c.File.Roles.Any(x => user.IsInRole(x)) == false) continue;

                yield return c.Name;
            }
        }

        /// <summary>
        /// Load specific component by name
        /// </summary>
        internal ComponentInfo Load(IPrincipal user, IServiceProvider service, string name)
        {
            if (_components.TryGetValue(name, out var c))
            {
                if (c.IsAutenticated && user.Identity.IsAuthenticated == false) return ComponentInfo.Error(name, new HttpException(401));
                if (c.Roles.Length > 0 && c.Roles.Any(x => user.IsInRole(x)) == false) return ComponentInfo.Error(name, new HttpException(403));

                return c;
            }
            else if(_discovers.TryGetValue(name, out var d))
            {
                var loader = new ComponentLoader(service, _compilers);

                c = loader.Load(d, this.JsonSettings);

                _components[c.Name] = c;

                if (c.IsAutenticated && user.Identity.IsAuthenticated == false) return ComponentInfo.Error(name, new HttpException(401));
                if (c.Roles.Length > 0 && c.Roles.Any(x => user.IsInRole(x)) == false) return ComponentInfo.Error(name, new HttpException(403));

                return c;
            }
            else
            {
                return ComponentInfo.Error(name, new Exception($"Component {name} not exists"));
            }
        }
    }
}

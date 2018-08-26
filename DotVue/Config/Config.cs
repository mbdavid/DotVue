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

namespace DotVue
{
    public class Config
    {
        private readonly Dictionary<string, ComponentDiscover> _discovers = new Dictionary<string, ComponentDiscover>();

        private readonly Dictionary<string, ComponentInfo> _components = new Dictionary<string, ComponentInfo>();

        public Config()
        {
        }

        /// <summary>
        /// Get/Set extension used in Vue components. Default: ".vue"
        /// </summary>
        public string Extension { get; set; } = ".vue";

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
                if (c.File.Auth && user.Identity.IsAuthenticated == false) continue;
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
                if (c.Roles.Length > 0 && c.Roles.Any(x => user.IsInRole(x)) == false) ComponentInfo.Error(name, new HttpException(403));

                return c;
            }
            else if(_discovers.TryGetValue(name, out var d))
            {
                var loader = new ComponentLoader(service);

                c = loader.Load(d);

                _components[c.Name] = c;

                return c;
            }
            else
            {
                return ComponentInfo.Error(name, new Exception($"Component {name} not exists"));
            }
        }
    }
}

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
        private readonly Dictionary<string, ComponentInfo> _components = new Dictionary<string, ComponentInfo>();

        private readonly IServiceProvider _service;
        private readonly bool _isDev;

        public Config(IServiceProvider service)
        {
            _service = service;

            var env = _service.GetService<IHostingEnvironment>();

            _isDev = env.IsDevelopment();
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
            var loader = new ComponentLoader();

            foreach (var path in assembly
                .GetManifestResourceNames()
                .Where(x => Path.GetExtension(x) == Extension))
            {
                using (var stream = assembly.GetManifestResourceStream(path))
                {
                    var component = loader.Load(path, stream, assembly);

                    _components[component.Name] = component;
                }
            }
        }

        /// <summary>
        /// Return all component that user has access
        /// </summary>
        internal IEnumerable<string> Discover(IPrincipal user, string root)
        {
            // in development, read physical root directory and find pages (not only embedded resources)
            if (root != null)
            {
                this.LoadWebFilesComponents(root);
            }

            foreach (var c in _components.Values)
            {
                if (c.IsAutenticated && user.Identity.IsAuthenticated == false) continue;
                if (c.Roles.Length > 0 && c.Roles.Any(x => user.IsInRole(x)) == false) continue;

                yield return c.Name;
            }
        }

        /// <summary>
        /// Load specific component by name
        /// </summary>
        internal ComponentInfo Load(IPrincipal user, string name)
        {
            if (_components.TryGetValue(name, out var c))
            {
                if (c.IsAutenticated && user.Identity.IsAuthenticated == false) return ComponentInfo.Message(name, $"Component '{name}' requires authentication");;
                if (c.Roles.Length > 0 && c.Roles.Any(x => user.IsInRole(x)) == false) ComponentInfo.Message(name, $"Component '{name}' requires roles '{string.Join(", ", c.Roles)}'");

                return c;
            }
            else
            {
                return ComponentInfo.Message(name, $"Component '{name}' not found");
            }
        }

        /// <summary>
        /// Load all from webroot path (for debug mode)
        /// </summary>
        private void LoadWebFilesComponents(string root)
        {
            var webAssembly = Assembly.GetEntryAssembly();
            var loader = new ComponentLoader();

            foreach (var path in Directory.GetFiles(root, "*" + Extension, SearchOption.AllDirectories))
            {
                using (var stream = File.OpenRead(path))
                {
                    var name = path.Replace(@"\", ".");

                    try
                    {
                        var component = loader.Load(name, stream, webAssembly);

                        _components[component.Name] = component;
                    }
                    catch(Exception ex)
                    {
                        _components[name] = ComponentInfo.Message(name, $"Error on load '{name}': {ex.Message}<pre>{ex.StackTrace}</pre>");
                    }
                }
            }
        }
    }
}

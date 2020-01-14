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
        private readonly Assembly[] _assemblies;
        private readonly Dictionary<string, ComponentInfo> _components = new Dictionary<string, ComponentInfo>(StringComparer.OrdinalIgnoreCase);
        private readonly StringBuilder _globalScripts = new StringBuilder();

        public Config(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        /// <summary>
        /// Load all componentes into _components memory
        /// </summary>
        internal void Discover(IServiceProvider service)
        {
            var loader = new ComponentLoader(service);

            // reading all html files inside assemblies
            foreach (var assembly in _assemblies)
            {
                var files = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".vue"));

                foreach (var file in files)
                {
                    try
                    {
                        var component = loader.Load(assembly, file);

                        _components[component.Name] = component;
                    }
                    catch (Exception ex)
                    {
                        _globalScripts.Append($"document.body.innerHTML = `<h1>{ex.Message.Replace("`", "'")}</h1><h3>{file}</h3><pre>{ex.StackTrace}</pre>`");

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Return component by name
        /// </summary>
        internal ComponentInfo GetComponent(string name)
        {
            if (_components.TryGetValue(name, out var component))
            {
                return component;
            }
            else
            {
                throw new Exception("Component `{name}` not found");
            }
        }

        /// <summary>
        /// Return all registered components
        /// </summary>
        internal ICollection<ComponentInfo> GetComponents()
        {
            return _components.Values;
        }

        /// <summary>
        /// Return global script injection
        /// </summary>
        internal StringBuilder GlobalScripts => _globalScripts;
    }
}

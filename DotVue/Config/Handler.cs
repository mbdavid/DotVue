using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DotVue
{
    public class Handler
    {
        private readonly Config _config;
        private readonly RequestDelegate _next;

        public Handler(RequestDelegate next, Config config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var user = context.User;

            var path = context.Request.Path.Value;
            var name = Path.GetFileNameWithoutExtension(path);

            var isBootstrap = path.EndsWith("/bootstrap.vue");
            var isLoad = request.Method == "GET";
            var isPost = request.Method == "POST";

            if (isBootstrap)
            {
                response.ContentType = "text/javascript";

                _config.Discover(context.RequestServices);

                var writer = new StringBuilder();
                var render = new ComponentRender(writer);

                writer.Append("(function() {\n");

                // inject dot-vue.js
                writer.Append(new StreamReader(typeof(Handler)
                    .Assembly
                    .GetManifestResourceStream("DotVue.Scripts.dot-vue.js"))
                    .ReadToEnd());

                // inject global script
                writer.Append(_config.GlobalScripts);

                writer.Append("//\n// Create Vue Components\n//\n");

                foreach (var component in _config.GetComponents().Where(x => !string.IsNullOrWhiteSpace(x.Template)))
                {
                    writer.Append($"const {component.Name} = ");

                    if (component.IsAsync)
                    {
                        writer.Append($"DotVue.async('{component.Name}')");
                    }
                    else
                    {
                        render.RenderComponent(component);
                    }

                    writer.Append($";\n");
                }

                writer.Append("\n//\n// Registering Vue Components\n//\n");

                render.RenderRegister(_config.GetComponents().Where(x => x.IsPage == false && !string.IsNullOrWhiteSpace(x.Template)));

                writer.Append("\n//\n// Registering Vue Routes\n//\n");

                render.RenderRoutes(_config.GetComponents().Where(x => x.IsPage == true && !string.IsNullOrWhiteSpace(x.Template)).ToList());

                writer.Append("\n//\n// Registering Styles\n//\n");

                render.RenderStyles(_config.GetComponents());

                writer.Append("})();");

                await response.WriteAsync(writer.ToString());
            }
            else if (isLoad)
            {
                response.ContentType = "text/javascript";

                _config.Discover(context.RequestServices);
                
                // render component script
                var component = _config.GetComponent(name);
                var writer = new StringBuilder();
                var render = new ComponentRender(writer);
                
                render.RenderComponent(component);

                await response.WriteAsync(writer.ToString());
            }
            else if (isPost)
            {
                response.ContentType = "text/json";
                
                // execute component update
                var data = request.Form["data"];
                var props = request.Form["props"];
                var method = request.Form["method"];
                var parameters = JArray.Parse(request.Form["params"]).ToArray();

                var component = _config.GetComponent(name);

                // check for autentication at viewModel level
                if (component.IsAuthenticated && context.User.Identity.IsAuthenticated == false) throw new HttpException(401);
                if (component.Roles.Length > 0 && component.Roles.Any(x => context.User.IsInRole(x)) == false) throw new HttpException(403, $"Forbidden. This view model requires all this roles: `{string.Join("`, `", component.Roles)}`");

                var update = new ComponentUpdate(component, user);

                var writer = new StreamWriter(response.Body);

                var vm = (ViewModel)ActivatorUtilities.CreateInstance(context.RequestServices, component.ViewModelType);

                // adding reference to viewmodel
                var vueContext = context.RequestServices.GetService<IVueContext>();

                vueContext.HttpContext = context;
                vueContext.ViewModel = vm;
                vueContext.Method = method;

                await update.UpdateModel(vm, data, props, method, parameters, context, writer);
            }
        }
    }
}

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
                
                var writer = new StringBuilder();
                var render = new ComponentRender(writer);

                writer.Append("(function() {\n");

                writer.Append("//\n// Create Vue Components\n//\n");

                // discover all components
                _config.Discover(context.RequestServices);

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

                try
                {
                    var component = _config.GetComponent(name);

                    var update = new ComponentUpdate(component, user);

                    var writer = new StreamWriter(response.Body);

                    var vm = (ViewModel)ActivatorUtilities.CreateInstance(context.RequestServices, component.ViewModelType);

                    await update.UpdateModel(vm, data, props, method, parameters, request.Form.Files, writer);
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException) ex = ex.InnerException;

                    response.StatusCode = 500;

                    await response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        type = ex.GetType().Name,
                        message = ex.Message,
                        source = ex.Source,
                        stacktrace = ex.StackTrace
                    }));
                }
            }
        }
    }
}

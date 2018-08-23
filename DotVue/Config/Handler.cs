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

namespace DotVue
{
    public class Handler
    {
        private readonly IHostingEnvironment _env;
        private readonly RequestDelegate _next;
        private readonly Config _config;

        public Handler(IHostingEnvironment env, RequestDelegate next, Config config)
        {
            _env = env;
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

                // output javascript lib
                var js = new StreamReader(typeof(Handler)
                    .Assembly
                    .GetManifestResourceStream("DotVue.Scripts.dot-vue.js"))
                    .ReadToEnd();

                var writer = new StringBuilder(js);

                writer.Append("\n\n//\n// Registering Vue Components\n//\n");

                // get root path only in development
                var root = _env.IsDevelopment() ? _env.ContentRootPath : null;

                foreach (var comp in _config.Discover(user, root))
                {
                    writer.Append($"Vue.component('{comp}', Vue.$loadComponent('{comp}'));\n");
                }

                await response.WriteAsync(writer.ToString());
            }
            else if (isLoad)
            {
                response.ContentType = "text/javascript";

                // render component script
                var component = _config.Load(user, name);
                var render = new ComponentRender(component, user);
                var sb = new StringBuilder();

                render.RenderScript(sb);

                await response.WriteAsync(sb.ToString());
            }
            else if (isPost)
            {
                response.ContentType = "text/json";

                // execute component update
                var data = request.Form["data"];
                var props = request.Form["props"];
                var method = request.Form["method"];
                var parameters = JArray.Parse(request.Form["params"]).ToArray();

                var component = _config.Load(user, name);

                var update = new ComponentUpdate(component, user);

                var writer = new StreamWriter(response.Body);

                try
                {
                    await update.UpdateModel(data, props, method, parameters, request.Form.Files, writer);
                }
                catch(Exception ex)
                {
                    var err = ex is TargetInvocationException ? ex.InnerException : ex;

                    response.Clear();
                    response.ContentType = "text/html";
                    response.StatusCode = 500;
                    await response.WriteAsync(
                        $"<div style='background-color: white; border: 1px solid gray; padding: 20px; font-family: Arial; font-size: 11px; color: black;'>" +
                        $"<div style='color: #800000; font-size: 22px; font-style: italic;'>{err.Message}</div>" +
                        $"<div style='margin-top: 20px;'><strong>Component:</strong> {name} - <strong>Method:</strong> {method}(<code style='font-family: Consolas; background-color: #eff0f1; padding: 1px 5px;'>{request.Form["params"]}</code>)</div>" +
                        $"<div style='margin-top: 20px;'><strong>Data:</strong> <code style='font-family: Consolas; background-color: #eff0f1; padding: 1px 5px;'>{data}</code></div>" +
                        $"<pre style='margin-top: 20px; background-color: #ffc; padding: 20px;'>{err.StackTrace}</pre>" +
                        $"</div>");
                }
            }
        }
    }
}

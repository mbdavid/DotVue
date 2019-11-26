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
                
                // output javascript lib
                var js = new StreamReader(typeof(Handler)
                    .Assembly
                    .GetManifestResourceStream("DotVue.Scripts.dot-vue.js"))
                    .ReadToEnd();
                
                var writer = new StringBuilder(js);
                
                writer.Append("\n\n//\n// Registering Vue Components\n//\n");

                // discover all components
                _config.Discover(context.RequestServices);

            }
            else if (isLoad)
            {
                //response.ContentType = "text/javascript";
                //
                //// render component script
                //var component = _config.Load(user, context.RequestServices, name);
                //var render = new ComponentRender(component, user);
                //var sb = new StringBuilder();
                //
                //render.RenderScript(sb);
                //
                //await response.WriteAsync(sb.ToString());
            }
            else if (isPost)
            {
                //response.ContentType = "text/json";
                //
                //// execute component update
                //var data = request.Form["data"];
                //var props = request.Form["props"];
                //var method = request.Form["method"];
                //var parameters = JArray.Parse(request.Form["params"]).ToArray();
                //
                //var component = _config.Load(user, context.RequestServices, name);
                //
                //var update = new ComponentUpdate(component, user);
                //
                //var writer = new StreamWriter(response.Body);
                //
                //var vm = (ViewModel)ActivatorUtilities.CreateInstance(context.RequestServices, component.ViewModelType);
                //
                //await update.UpdateModel(vm, data, props, method, parameters, request.Form.Files, writer);
            }
        }
    }
}

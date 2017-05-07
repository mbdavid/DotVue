using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    public class Handler : IHttpHandler
    {
        /// <summary>
        /// Define custom vue file loader
        /// </summary>
        public static List<IComponentLoader> Loaders { get; private set; } = new List<IComponentLoader>()
        {
            new AscxLoader(),
            new StaticLoader()
        };

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            var path = context.Request.FilePath;
            var isBootstrap = path.EndsWith("/bootstrap.vue");
            var isLoad = context.Request.HttpMethod == "GET";
            var isPost = context.Request.HttpMethod == "POST";

            context.Response.ContentType = "text/javascript";

            if (isBootstrap)
            {
                // output javascript lib
                typeof(Handler)
                    .Assembly
                    .GetManifestResourceStream("DotVue.Scripts.dot-vue.js")
                    .CopyTo(context.Response.OutputStream);

                var discover = context.Request.QueryString["discover"];

                if (string.IsNullOrEmpty(discover)) return;

                context.Response.Output.Write("\n\n//\n");
                context.Response.Output.Write("// Registering Vue Components\n");
                context.Response.Output.Write("//");

                // register all components with async load
                foreach (var loader in Loaders)
                {
                    context.Response.Output.Write("\n\n// Loader: " + loader.GetType().Name);

                    foreach (var c in loader.Discover(context))
                    {
                        context.Response.Output.Write("\nVue.component('{0}', Vue.$loadComponent(", c.Name);

                        if (discover == "sync")
                        {
                            var component = loader.Load(context, c.VPath);
                            context.Response.Output.Write("function() {\n");
                            component.RenderScript(context.Response.Output);
                            context.Response.Output.Write("}");
                        }
                        else // async
                        {
                            context.Response.Output.WriteFormat("'{0}'", c.VPath);
                        }

                        context.Response.Output.Write("));");
                    }
                }
            }
            else if(isLoad)
            {
                // render component script
                Load(context, path)
                    .RenderScript(context.Response.Output);
            }
            else if(isPost)
            {
                // execute component update
                var request = context.Request;
                var response = context.Response;

                var data = request.Form["data"];
                var props = request.Form["props"];
                var method = request.Form["method"];
                var parameters = JArray.Parse(request.Form["params"]).ToArray();
                var files = request.Files.GetMultiple("files");

                var component = Load(context, path);

                component.UpdateModel(data, props, method, parameters, files, response.Output);

                response.ContentType = "text/json";
            }
        }

        /// <summary>
        /// Load component from any loaders and cache result
        /// </summary>
        private Component Load(HttpContext context, string path)
        {
            foreach(var l in Loaders)
            {
                var c = l.Load(context, path);

                if(c != null) return c;
            }

            throw new HttpException("Vue component [" + path + "] not found");
        }
    }
}

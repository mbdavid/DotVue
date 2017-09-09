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
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;

            var path = request.FilePath;
            var isBootstrap = path.EndsWith("/bootstrap.vue");
            var isLoad = request.HttpMethod == "GET";
            var isPost = request.HttpMethod == "POST";
            var output = response.Output;

            response.ContentType = "text/javascript";

            if (isBootstrap)
            {
                // output javascript lib
                typeof(Handler)
                    .Assembly
                    .GetManifestResourceStream("DotVue.Scripts.dot-vue.js")
                    .CopyTo(response.OutputStream);

                var discover = request.QueryString["discover"];

                if (string.IsNullOrEmpty(discover)) return;

                output.Write("\n\n//\n");
                output.Write("// Registering Vue Components\n");
                output.Write("//");

                // register all components with async load
                foreach (var loader in Config.Instance.Loaders)
                {
                    output.Write("\n\n// Loader: " + loader.GetType().Name);

                    foreach (var c in loader.Discover(context))
                    {
                        output.Write("\nVue.component('{0}', Vue.$loadComponent(", c.Name);

                        if (discover == "sync")
                        {
                            var component = loader.Load(context, c.VPath);
                            var plugins = loader.Plugins(context, component.Name)
                                .Select(x => loader.Load(context, x))
                                .Where(x => Config.Instance.Install(context, component.Name, x.Name));

                            output.Write("function() {\n");
                            new Component(component, plugins).RenderScript(output);
                            output.Write("}");
                        }
                        else // async
                        {
                            output.WriteFormat("'{0}'", c.VPath);
                        }

                        output.Write("));");
                    }
                }
            }
            else if(isLoad)
            {
                // render component script
                this.Load(context, path)
                    .RenderScript(output);
            }
            else if(isPost)
            {
                // execute component update
                var data = request.Form["data"];
                var props = request.Form["props"];
                var method = request.Form["method"];
                var parameters = JArray.Parse(request.Form["params"]).ToArray();

                var component = this.Load(context, path);

                component.UpdateModel(data, props, method, parameters, request.Files, output);

                response.ContentType = "text/json";
            }
        }

        /// <summary>
        /// Load component from any loaders and cache result
        /// </summary>
        private Component Load(HttpContext context, string vpath)
        {
            foreach(var loader in Config.Instance.Loaders)
            {
                var component = loader.Load(context, vpath);                

                if(component != null)
                {
                    var plugins = loader
                        .Plugins(context, component.Name)
                        .Select(x => loader.Load(context, x))
                        .Where(x => Config.Instance.Install(context, component.Name, x.Name));

                    return new Component(component, plugins);
                }
            }

            throw new HttpException("Vue component [" + vpath + "] not found");
        }
    }
}

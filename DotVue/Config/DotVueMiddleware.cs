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
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DotVue
{
    public static class DotVueMiddlewareExtensions
    {
        public static IApplicationBuilder UseDotVue(this IApplicationBuilder builder, Action<Config> setup)
        {
            var config = new Config();

            setup(config);

            return builder.MapWhen(c => c.Request.Path.Value.EndsWith(".vue"), app =>
            {
                app.UseMiddleware<Handler>(config);
            });
        }

        public static IApplicationBuilder UseDotVue(this IApplicationBuilder builder, params Assembly[] assemblies)
        {
            return UseDotVue(builder, c =>
            {
                foreach(var a in assemblies)
                {
                    c.AddAssembly(a);
                }
            });
        }
    }
}

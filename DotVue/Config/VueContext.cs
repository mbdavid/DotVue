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
using Microsoft.AspNetCore.Http;

namespace DotVue
{
    public class VueContext : IVueContext
    {
        public HttpContext HttpContext { get; set; }

        public ViewModel ViewModel { get; set; }

        public string Method { get; set; }
    }
}

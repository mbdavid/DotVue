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
    internal class ComponentDiscover
    {
        public string Name { get; set; }
        public Assembly Assembly { get; set; }
        public HtmlFile File { get; set; }
    }
}

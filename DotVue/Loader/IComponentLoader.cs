using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    public interface IComponentLoader
    {
        IEnumerable<ComponentInfo> Discover(HttpContext context);
        IEnumerable<string> Plugins(HttpContext context, string componentName);
        ComponentInfo Load(HttpContext context, string vpath);
    }
}

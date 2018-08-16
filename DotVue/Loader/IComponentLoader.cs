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
        IEnumerable<ComponentDiscover> Discover(HttpContext context);
        ComponentInfo Load(HttpContext context, string vpath);
    }
}

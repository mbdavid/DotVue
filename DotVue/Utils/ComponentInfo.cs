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
    public class ComponentInfo
    {
        public string VPath { get; set; }
        public string Name { get; set; }
    }
}

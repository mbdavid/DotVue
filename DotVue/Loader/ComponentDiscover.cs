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
    public class ComponentDiscover
    {
        public string Name { get; private set; }
        public string VPath { get; private set; }

        public ComponentDiscover(string name, string vpath)
        {
            this.Name = name;
            this.VPath = vpath;
        }
    }
}

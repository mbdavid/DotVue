using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    public class VueMethod
    {
        public string Name { get; set; }
        public string PreScript { get; set; }
        public string PostScript { get; set; }
        public List<string> Parameters { get; set; } = new List<string>();
    }
}

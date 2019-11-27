using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    internal class ComponentInfo
    {
        public string Name { get; }

        public bool IsPage { get; set; }
        public string Route { get; set; }
        public bool IsAsync { get; set; }
        public bool InheritAttrs { get; set; } = true;

        public Type ViewModelType { get; set; } = typeof(ViewModel);

        public string Template { get; set; }
        public string Styles { get; set; }
        public string Scripts { get; set; }
        public List<string> Mixins { get; set; } = new List<string>();

        public JObject JsonData { get; set; } = new JObject();
        public Dictionary<string, ViewModelMethod> Methods { get; set; } = new Dictionary<string, ViewModelMethod>();

        public Dictionary<string, object> Props { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> RouteParams { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> QueryString { get; set; } = new Dictionary<string, object>();

        public bool IsAuthenticated { get; set; }
        public string[] Roles { get; set; } = new string[0];

        public bool CreatedHook => this.Methods.ContainsKey("OnCreated");

        public ComponentInfo(string name)
        {
            this.Name = Regex.Replace(Regex.Replace(name, @"^\d*", ""), @"\s", "_");
        }
    }
}

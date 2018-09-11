using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    internal class ComponentInfo
    {
        public string Name { get; set; }

        public Type ViewModelType { get; set; } = typeof(ViewModel);

        public bool IsAutenticated { get; set; } = false;
        public string[] Roles { get; set; } = new string[0];

        public string Template { get; set; } = "";
        public bool InheritAttrs { get; set; } = true;
        public List<string> Styles { get; set; } = new List<string>();
        public List<string> Scripts { get; set; } = new List<string>();
        public List<string> Includes { get; set; } = new List<string>();

        public string JsonData { get; set; } = "{}";
        public List<string> Props { get; set; } = new List<string>();
        public Dictionary<string, ViewModelMethod> Methods { get; set; } = new Dictionary<string, ViewModelMethod>();
        public Dictionary<string, string> Computed { get; set; } = new Dictionary<string, string>();
        public List<string> Locals { get; set; } = new List<string>();

        public bool CreatedHook => this.Methods.ContainsKey("OnCreated");

        public static ComponentInfo Error(string name, Exception ex)
        {
            return new ComponentInfo
            {
                Name = name,
                Template = 
                    $"<div style=\"background-color:#ffc;padding:15px;font-family:Arial;font-size:12px;\">" + 
                    $"<div style='color: #800000; font-size: 22px; font-style: italic;'>[{name}.vue] [{ex.GetType().Name}] {ex.Message}</div>" + 
                    $"<pre>{ex.StackTrace}</pre>" +
                    "</div>"
            };
        }
    }
}

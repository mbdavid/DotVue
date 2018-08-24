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
        public List<string> Styles { get; set; } = new List<string>();
        public List<string> Scripts { get; set; } = new List<string>();
        public List<string> Includes { get; set; } = new List<string>();

        public JObject Data { get; set; } = new JObject();
        public List<string> Props { get; set; } = new List<string>();
        public Dictionary<string, ViewModelMethod> Methods { get; set; } = new Dictionary<string, ViewModelMethod>();
        public Dictionary<string, string> Watch { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Computed { get; set; } = new Dictionary<string, string>();
        public List<string> Locals { get; set; } = new List<string>();

        public bool CreatedHook => this.Methods.ContainsKey("OnCreated");

        public static ComponentInfo Message(string name, string message)
        {
            return new ComponentInfo
            {
                Name = name,
                Template = $"<div class='dot-vue-error'>{message}</div>"
            };
        }
    }
}

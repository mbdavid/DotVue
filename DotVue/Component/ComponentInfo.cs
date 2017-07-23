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
        public string VPath { get; private set; }
        public string Name { get; private set; }
        public Type ViewModel { get; private set; }
        public string Content { get; private set; }

        public ComponentInfo(string name, string vpath)
        {
            this.Name = name;
            this.VPath = vpath;
        }

        public ComponentInfo(string name, string vpath, Type viewModel, string content)
        {
            this.Name = name;
            this.VPath = vpath;
            this.ViewModel = viewModel;
            this.Content = content;
        }
    }
}

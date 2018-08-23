using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotVue
{
    internal class ViewModelMethod
    {
        public string[] Pre { get; set; }
        public string[] Post { get; set; }
        public string[] Parameters { get; set; }
        public MethodInfo Method { get; set; }
    }
}

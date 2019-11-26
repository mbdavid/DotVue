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
        public string[] Parameters { get; set; }
        public MethodInfo Method { get; set; }
        public string Watch { get; set; }

        public bool IsAuthenticated { get; set; } = false;
        public string[] Roles { get; set; } = new string[0];
    }
}

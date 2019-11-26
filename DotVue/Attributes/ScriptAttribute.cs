using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Inject script in "created" function
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class ScriptAttribute : Attribute
    {
        public virtual string Code { get; }

        public ScriptAttribute(string code)
        {
            this.Code = code;
        }
    }
}

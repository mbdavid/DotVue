using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Execute script pre/post $update be called
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScriptAttribute : Attribute
    {
        public string Pre { get; set; }
        public string Post { get; set; }

        public ScriptAttribute(string pre)
        {
            this.Pre = pre;
        }

        public ScriptAttribute(string pre, string post)
        {
            this.Pre = pre;
            this.Post = post;
        }
    }
}

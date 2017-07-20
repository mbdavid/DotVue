using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Execute script pre/post $update be called
    /// </summary>
    public class ScriptAttribute : Attribute
    {
        public string Pre { get; set; }
        public string Post { get; set; }

        public ScriptAttribute(string pre)
        {
            Pre = pre;
        }

        public ScriptAttribute(string pre, string post)
        {
            Pre = pre;
            Post = post;
        }
    }

    public class ConfirmAttribute : ScriptAttribute
    {
        public ConfirmAttribute(string text)
            : base("if (confirm('" + text.EncodeJavascript() + "') === false) return false;")
        {
        }
    }

    /// <summary>
    /// Define C# class property as an Vue props - do not update this value in server-side (it´s updated by parent component only)
    /// </summary>
    public class PropAttribute : Attribute
    {
    }

    /// <summary>
    /// Define variable name to subscribe for changes on client side
    /// </summary>
    public class WatchAttribute : Attribute
    {
        public string Name { get; set; }

        /// <summary>
        /// Define variable name to subscribe for changes on client side
        /// </summary>
        public WatchAttribute(string name)
        {
            Name = name;
        }
    }
}

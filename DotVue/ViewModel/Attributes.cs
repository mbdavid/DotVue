using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Execute script pre/post $update be called
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
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

    /// <summary>
    /// Add custom 'confirm' script before send data to server
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
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
    [AttributeUsage(AttributeTargets.Property)]
    public class PropAttribute : Attribute
    {
    }

    /// <summary>
    /// Define variable name to subscribe for changes on client side
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class WatchAttribute : Attribute
    {
        public string Name { get; private set; }

        /// <summary>
        /// Define variable name to subscribe for changes on client side
        /// </summary>
        public WatchAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Define property as client only data, do not send from client to server (will be sent only server to client)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalAttribute : Attribute
    {
    }
}

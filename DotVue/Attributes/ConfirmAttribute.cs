using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
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
}

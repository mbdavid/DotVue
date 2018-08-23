using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
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
            this.Name = name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Represent a computed 
    /// </summary>
    public class Computed
    {
        /// <summary>
        /// Javascript code after resolve expression
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// Func to return compiled expression
        /// </summary>
        public Func<object, object> Value { get; internal set; }

        internal Computed()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Define is user has permission to load/run component/method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutorizeAttribute : Attribute
    {
        public string[] Roles { get; set; }

        public AutorizeAttribute()
        {
            this.Roles = new string[0];
        }

        public AutorizeAttribute(params string[] roles)
        {
            this.Roles = roles;
        }
    }
}

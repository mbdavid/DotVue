using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Define C# property as client-side persisted (use localStorage)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalStorageAttribute : Attribute
    {
        public string Key { get; set; }

        public LocalStorageAttribute(string key)
        {
            this.Key = key;
        }

        public LocalStorageAttribute()
        {
        }
    }
}

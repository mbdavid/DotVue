using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Define C# class property as an Vue props - do not update this value in server-side (it´s updated by parent component only)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : Attribute
    {
        public string Code { get; private set; }

        public ComputedAttribute(string fnCode)
        {
            this.Code = $"return {fnCode};";
        }

        public ComputedAttribute(string thisVar, string fnCode)
        {
            this.Code = $"return (function({thisVar}) {{ return {fnCode}; }})(this)";
        }
    }
}

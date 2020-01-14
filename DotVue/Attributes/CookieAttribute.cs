using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Define C# class field as an browser cookie - send back to browser current value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CookieAttribute : Attribute
    {
    }
}

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
    public class PropAttribute : Attribute
    {
    }
}

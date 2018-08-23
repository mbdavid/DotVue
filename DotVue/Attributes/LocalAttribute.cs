using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Define property as client only data, do not send from client to server (will be sent only server to client)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalAttribute : Attribute
    {
    }
}

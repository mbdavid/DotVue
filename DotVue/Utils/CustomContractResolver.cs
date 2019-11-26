using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotVue
{
    internal class CustomContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x as MemberInfo);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<RouteParamAttribute>(true) != null || x.GetCustomAttribute<QueryStringAttribute>(true) != null)
                .Select(x => x as MemberInfo);

            var props = properties
                .Union(fields)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .ToList();

            props.ForEach(p => { p.Writable = true; p.Readable = true; });

            return props;
        }
    }
}

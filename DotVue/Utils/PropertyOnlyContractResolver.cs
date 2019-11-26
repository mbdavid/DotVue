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
    internal class PropertyOnlyContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .ToList();

            props.ForEach(p => { p.Writable = true; p.Readable = true; });

            return props;
        }
    }
}

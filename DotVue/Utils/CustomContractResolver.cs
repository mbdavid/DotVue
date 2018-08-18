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
    internal class CustomContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly CustomContractResolver Instance = new CustomContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            // ignore Computed property
            var props = base.CreateProperties(type, memberSerialization)
                .Where(x => x.AttributeProvider.GetAttributes(typeof(ComputedAttribute), true).Count == 0)
                .ToList();

            // props must be write-only
            props.ForEach(x =>
            {
                if(x.AttributeProvider.GetAttributes(true).Any(z => z.GetType() == typeof(PropAttribute)))
                {
                    x.Readable = false;
                    x.Writable = true;
                }
            });

            return props;
        }
    }
}

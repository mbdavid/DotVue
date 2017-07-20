using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    public class Plugin : IDisposable
    {
        /// <summary>
        /// Function to be override to set if some plugin must be used or not
        /// </summary>
        public static Func<string, bool> Install = (name) => true;

        protected JObject ViewModelData { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Sections { get; set; } = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        [JsonIgnore]
        public string Style { get; set; }

        [JsonIgnore]
        public string Script { get; set; }

        public Plugin(string content)
        {
        }

        public void Initialize(JObject data)
        {
            this.ViewModelData = data;
        }

        public virtual void OnExecuting(MethodInfo method, List<object> parameters)
        {
        }

        public virtual void OnExecuted(MethodInfo method, List<object> parameters)
        {
        }

        public virtual void Dispose()
        {
        }

    }
}

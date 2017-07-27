using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    public class Config
    {
        #region Static instance

        private static object _locker = new object();
        private static Config _instance = null;

        internal static Config Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new Config();
                        Component.RunSetup(_instance);
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Json.Net settings

        internal static JsonSerializer JSettings = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = CustomContractResolver.Instance
        };

        internal static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = CustomContractResolver.Instance
        };

        internal static JsonMergeSettings MergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        #endregion

        /// <summary>
        /// Get or add new compiler for string content tags (scripts/styles or template)
        /// </summary>
        public Dictionary<string, Func<string, string>> Compilers { get; private set; } = new Dictionary<string, Func<string, string>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Define custom vue file loader
        /// </summary>
        public List<IComponentLoader> Loaders { get; private set; } = new List<IComponentLoader>()
        {
            new AscxLoader()
            //new StaticLoader()
        };

        /// <summary>
        /// Install function to verify if some plugin must be installed or not. It will be called when need render component
        /// </summary>
        public Func<HttpContext, string, string, bool> Install { get; set; } = (context, component, plugin) => true;


        /// <summary>
        /// Execute compiler function according lang
        /// </summary>
        internal string RunCompiler(string lang, string content)
        {
            if (string.IsNullOrEmpty(lang)) return content;

            Func<string, string> compiler;

            if (this.Compilers.TryGetValue(lang, out compiler))
            {
                return compiler(content);
            }

            throw new ArgumentException("Compiler for language " + lang + " not defined. Use Component.RegisterCompiler");
        }
    }
}

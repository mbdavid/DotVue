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
        /// Define custom vue file loader
        /// </summary>
        public static List<IComponentLoader> Loaders { get; private set; } = new List<IComponentLoader>()
        {
            new AscxLoader()
            //new StaticLoader()
        };
    }
}

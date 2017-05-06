using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Implement component loader based on .vue file plus @ViewModel directive
    /// </summary>
    public class VueLoader : IComponentLoader
    {
        public IEnumerable<ComponentInfo> Discover(HttpContext context)
        {
            var ext = ".vue";
            var path = context.Server.MapPath("~/");
            var files = Directory.GetFiles(path, "*" + ext, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                yield return new ComponentInfo
                {
                    Name = Regex.Replace(Path.GetFileNameWithoutExtension(file), "^_+", ""),
                    VPath = file.Substring(path.Length - 1, file.Length - path.Length - ext.Length + 1).Replace('\\', '/') + ".vue"
                };
            }
        }

        private Regex _reViewModel = new Regex(@"@ViewModel\s+(?<name>[\w\.\,= `\<\>]+)", RegexOptions.IgnoreCase);

        public Component Load(HttpContext context, string vpath)
        {
            var file = context.Server.MapPath("~" + vpath);

            try
            {
                var content = File.ReadAllText(file);
                var viewModelName = _reViewModel.Match(content).Groups["name"].Value ?? "";
                var viewModelType = typeof(EmptyViewModel);

                if(viewModelName != null)
                {
                    if (viewModelName.Contains(","))
                    {
                        viewModelType = Type.GetType(viewModelName, true, true);
                    }
                    else
                    {
                        viewModelType = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(x => x.GetTypes())
                            .Where(x => x.FullName.Equals(viewModelName, StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault();

                        if (viewModelType == null) throw new Exception("ViewModel class not found: " + viewModelName);
                    }
                }

                return new Component(vpath, viewModelType, content.ToString());
            }
            catch (Exception ex)
            {
                return new Component(vpath, typeof(EmptyViewModel), "<template><div>" + ex.Message + "</div></template>");
            }
        }
    }
}

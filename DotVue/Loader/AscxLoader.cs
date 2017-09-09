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
    /// Implement component loader based on .ascx user control file in web application
    /// </summary>
    public class AscxLoader : IComponentLoader
    {
        public IEnumerable<ComponentInfo> Discover(HttpContext context)
        {
            var ext = ".ascx";
            var path = context.Server.MapPath("~/");
            var files = Directory.GetFiles(path, "*" + ext, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var name = Regex.Replace(Path.GetFileNameWithoutExtension(file), "^_+", "");
                var vpath = file.Substring(path.Length - 1, file.Length - path.Length - ext.Length + 1).Replace('\\', '/') + ".vue";

                // if name starts with $ is a plugin
                if (name.StartsWith("$")) continue;

                yield return new ComponentInfo(name, vpath);
            }
        }

        public IEnumerable<string> Plugins(HttpContext context, string componentName)
        {
            var ext = ".ascx";
            var path = context.Server.MapPath("~/");
            var files = Directory.GetFiles(path, "$" + componentName + "_*" + ext, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var vpath = file.Substring(path.Length - 1, file.Length - path.Length - ext.Length + 1).Replace('\\', '/') + ".vue";

                yield return vpath;
            }
        }

        public ComponentInfo Load(HttpContext context, string vpath)
        {
            var name = Regex.Replace(Path.GetFileNameWithoutExtension(vpath), "^_+", "");
            var ascx = "~/" + 
                vpath.Substring(1, vpath.Length - Path.GetExtension(vpath).Length - 1) +
                ".ascx";

            try
            {
                var loader = new UserControl();
                var content = new StringBuilder();
                var control = loader.LoadControl(ascx);

                // get viewmodel class type
                var viewModel = control.GetType()
                    .GetNestedTypes()
                    .Where(x => typeof(ViewModel).IsAssignableFrom(x))
                    .FirstOrDefault() ?? typeof(EmptyViewModel);

                using (var sw = new StringWriter(content))
                {
                    using (var w = new HtmlTextWriter(sw))
                    {
                        control.RenderControl(w);
                    }
                }

                return new ComponentInfo(name, vpath, viewModel, content.ToString());
            }
            catch (HttpCompileException ex)
            {
                var html = ex.GetHtmlErrorMessage();
                var re = new Regex(@"<pre[^>]*>\s*(?<content>[\s\S]*?)\s*<\/pre>");
                var code = re.Match(html).Groups["content"].Value;

                return new ComponentInfo(name, vpath, typeof(EmptyViewModel),
                    string.Format("<template><div><h3 style='color:#800000;'><i>{0}</i></h3><pre style='background-color: #FFFFCC'>{1}</pre></div></template>", ex.Message, code));
            }
            catch (HttpException)
            {
                return null; // file not found
            }
        }
    }
}

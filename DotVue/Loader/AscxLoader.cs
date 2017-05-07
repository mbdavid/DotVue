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
                yield return new ComponentInfo
                {
                    Name = Regex.Replace(Path.GetFileNameWithoutExtension(file), "^_+", ""),
                    VPath = file.Substring(path.Length - 1, file.Length - path.Length - ext.Length + 1).Replace('\\', '/') + ".vue"
                };
            }
        }

        public Component Load(HttpContext context, string vpath)
        {
            var ascx = "~/" + 
                vpath.Substring(1, vpath.Length - Path.GetExtension(vpath).Length - 1) +
                ".ascx";

            try
            {
                var loader = new UserControl();
                var content = new StringBuilder();
                var control = loader.LoadControl(ascx);

                using (var sw = new StringWriter(content))
                {
                    using (var w = new HtmlTextWriter(sw))
                    {
                        control.RenderControl(w);
                    }
                }

                // get viewmodel class type
                var viewModelType = control.GetType()
                    .GetNestedTypes()
                    .Where(x => typeof(ViewModel).IsAssignableFrom(x))
                    .FirstOrDefault() ?? typeof(EmptyViewModel);

                return new Component(vpath, viewModelType, content.ToString());
            }
            catch (HttpCompileException ex)
            {
                var html = ex.GetHtmlErrorMessage();
                var re = new Regex(@"<body[^>]*>\s*(?<content>[\s\S]*?)\s*<\/body>");
                var content = re.Match(html).Groups["content"];

                return new Component(vpath, typeof(EmptyViewModel), "<template><div>" + content + "</div></template>");
            }
            catch (HttpException)
            {
                return null; // file not found
            }
        }
    }
}

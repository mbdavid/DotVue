using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace DotVue
{
    /// <summary>
    /// Javascript builder class. Use this call to create extensions in your project with specific javascript code
    /// </summary>
    public class JavascriptBuilder
    {
        private StringBuilder _sb = new StringBuilder();

        public int Length => _sb.Length;

        /// <summary>
        /// Write text on console log
        /// </summary>
        public JavascriptBuilder ConsoleLog(string text)
        {
            return this.Code("console.log('{0}');", HttpUtility.JavaScriptStringEncode(text));
        }

        /// <summary>
        /// Show alert message box
        /// </summary>
        public JavascriptBuilder Alert(string text)
        {
            return this.Code("alert('{0}');", HttpUtility.JavaScriptStringEncode(text));
        }

        /// <summary>
        /// Set control focus - must define [ref='my-control'] in tag
        /// </summary>
        public JavascriptBuilder Focus(string refId)
        {
            return this.Code("this.$refs.{0}.focus();", refId);
        }

        /// <summary>
        /// Update location.href address
        /// </summary>
        public JavascriptBuilder RedirectTo(string url)
        {
            return this.Code("location.href = '{0}';", HttpUtility.JavaScriptStringEncode(url));
        }

        /// <summary>
        /// Reload location
        /// </summary>
        public JavascriptBuilder Reload()
        {
            return this.Code("location.reload();");
        }

        /// <summary>
        /// Trigger Vue event for parent component
        /// </summary>
        public JavascriptBuilder Emit(string eventName, params object[] args)
        {
            var sb = new StringBuilder("this.$emit('" + eventName +"'");

            foreach(var arg in args)
            {
                sb.Append(", " + JsonConvert.SerializeObject(arg));
            }

            sb.Append(");");

            return Code(sb.ToString());
        }

        /// <summary>
        /// Call any Vue method passing method and optional parameters
        /// </summary>
        public JavascriptBuilder Call(string methodName, params object[] args)
        {
            var sb = new StringBuilder("this." + methodName + ".call(this");

            foreach (var arg in args)
            {
                sb.Append(",");
                sb.Append(JsonConvert.SerializeObject(arg));
            }

            sb.Append(");");

            return this.Code(sb.ToString());
        }

        /// <summary>
        /// Add javascript code to be run when client Update finish
        /// </summary>
        public JavascriptBuilder Code(string code)
        {
            _sb.Append(code);
            return this;
        }

        public JavascriptBuilder Code(string format, params object[] args)
        {
            _sb.AppendFormat(format, args);
            return this;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}

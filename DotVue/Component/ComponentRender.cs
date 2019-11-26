//using Newtonsoft.Json;
//using System.IO;
//using System.Linq;
//using System.Security.Principal;
//using System.Text;

//namespace DotVue
//{
//    /// <summary>
//    /// Render component as javascript vue compoenent
//    /// </summary>
//    public class ComponentRender
//    {
//        private readonly ComponentInfo _component;
//        private readonly IPrincipal _user;

//        internal ComponentRender(ComponentInfo component, IPrincipal user)
//        {
//            _component = component;
//            _user = user;
//        }

//        public void RenderScript(StringBuilder writer)
//        {
//            writer.Append("//\n");
//            writer.AppendFormat("// Component: \"{0}\"\n", _component.Name);
//            writer.Append("//\n");

//            // add "style" before return Vue object
//            if (_component.Styles.Count > 0)
//            {
//                writer.AppendFormat("Vue.$addStyle('{0}');\n",
//                    string.Join("", _component.Styles.Select(x => x.EncodeJavascript())));
//            }

//            writer.Append("return {\n");
//            writer.AppendFormat(" includes: [{0}],\n", string.Join(",", _component.Includes.Select(x => $"'{x}'")));
//            writer.Append(" options: function() { return {\n");

//            if (_component.InheritAttrs == false)
//            {
//                writer.Append("  inheritAttrs: false,\n");
//            }

//            if (_component.Props.Count > 0)
//            {
//                writer.AppendFormat("  props: [{0}],\n", string.Join(", ", _component.Props.Select(x => "'" + x.ToCamelCase() + "'")));
//            }

//            // only call Created method if created was override in component
//            if (_component.CreatedHook)
//            {
//                writer.Append("  created: function() {\n");
//                writer.Append("    this.OnCreated();\n");
//                writer.Append("  },\n");
//            }

//            // append template string
//            writer.AppendFormat("  template: '{0}',\n", _component.Template.EncodeJavascript());
//            writer.AppendFormat("  data: function() {{\n    return {0};\n  }},\n", _component.JsonData);

//            // render methods
//            if (_component.Methods.Count > 0)
//            {
//                writer.Append("  methods: {\n");

//                foreach (var m in _component.Methods.Values)
//                {
//                    writer.AppendFormat("    {0}: function({1}) {{\n",
//                        m.Method.Name,
//                        string.Join(", ", m.Parameters));

//                    foreach(var script in m.Pre)
//                    {
//                        writer.AppendFormat("      {0}\n", script);
//                    }

//                    if (m.Post.Length > 0)
//                    {
//                        writer.Append("      var __vm = this;\n");
//                    }

//                    writer.AppendFormat("      return this.$update(this, '{0}', [{1}])",
//                        m.Method.Name,
//                        string.Join(", ", m.Parameters));

//                    if (m.Post.Length > 0)
//                    {
//                        writer.Append("\n        .then(function(__result) {");
//                        writer.Append("\n          (function() {");
//                        writer.AppendFormat("\n            {0}", string.Join("\n            ", m.Post));
//                        writer.Append("\n          }).call(__vm);");
//                        writer.Append("\n          return __result;");
//                        writer.Append("\n        });");
//                    }
//                    else
//                    {
//                        writer.Append(";");
//                    }

//                    writer.AppendFormat("\n    }}{0}\n", m == _component.Methods.Last().Value ? "" : ","); // method
//                }

//                writer.Append("  },\n"); // methods
//            }

//            // render def
//            if (_component.Computed.Count > 0)
//            {
//                writer.Append("  computed: {\n");

//                foreach (var c in _component.Computed)
//                {
//                    writer.AppendFormat("    {0}: function() {{\n      {1};\n    }}{2}\n",
//                        c.Key,
//                        c.Value,
//                        c.Key == _component.Computed.Last().Key ? "" : ",");
//                }

//                writer.Append("  },\n");
//            }

//            // render watchs
//            var watch = _component.Methods.Where(x => x.Value.Watch != null).ToArray();

//            if (watch.Length > 0)
//            {
//                writer.Append("  watch: {\n");

//                foreach (var w in watch)
//                {
//                    writer.AppendFormat("    {0}: {{\n      handler: function(v, o) {{\n        if (this.$updating) return false;\n        this.{1}(v, o);\n      }},\n      deep: true\n    }}{2}\n",
//                        w.Value.Watch, 
//                        w.Value.Method.Name,
//                        w.Key == watch.Last().Key ? "" : ",");
//                }

//                writer.Append("  },\n");
//            }

//            // render scripts
//            if (_component.Scripts.Count > 0)
//            {
//                writer.Append("  mixins: [\n");

//                foreach(var s in _component.Scripts)
//                {
//                    writer.AppendFormat("(function() {{\n{0}\n}})() || {{}}{1}\n",
//                        s,
//                        s == _component.Scripts.Last() ? "": ",");
//                }

//                writer.Append("  ],\n");
//            }

//            // render client-only properties
//            writer.AppendFormat("  local: [{0}],\n", string.Join(", ", _component.Locals.Select(x => "'" + x + "'")));

//            // add vpath to options
//            writer.AppendFormat("  name: '{0}'\n", _component.Name);

//            writer.Append(" }\n"); // return object
//            writer.Append("}\n"); // function
//            writer.Append("};"); // options
//        }
//    }
//}

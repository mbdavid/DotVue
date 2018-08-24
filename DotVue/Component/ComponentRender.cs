using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace DotVue
{
    /// <summary>
    /// Render component as javascript vue compoenent
    /// </summary>
    public class ComponentRender
    {
        private readonly ComponentInfo _component;
        private readonly IPrincipal _user;

        internal ComponentRender(ComponentInfo component, IPrincipal user)
        {
            _component = component;
            _user = user;
        }

        public void RenderScript(StringBuilder writer)
        {
            writer.Append("//\n");
            writer.AppendFormat("// Component: \"{0}\"\n", _component.Name);
            writer.Append("//\n");

            // add "style" before return Vue object
            if (_component.Styles.Count > 0)
            {
                writer.AppendFormat("Vue.$addStyle('{0}');\n",
                    string.Join("", _component.Styles.Select(x => x.EncodeJavascript())));
            }

            writer.Append("return {\n");
            writer.Append(" includes: [],\n");
            writer.Append(" options: {\n");

            if (_component.Props.Count > 0)
            {
                writer.AppendFormat("  props: [{0}],\n", string.Join(", ", _component.Props.Select(x => "'" + x.CamelCase() + "'")));
            }

            // only call Created method if created was override in component
            if (_component.CreatedHook)
            {
                writer.Append("  created: function() {\n");
                writer.Append("    this.OnCreated();\n");
                writer.Append("  },\n");
            }

            // append template string
            writer.AppendFormat("  template: '{0}',\n", _component.Template.EncodeJavascript());
            writer.AppendFormat("  data: function() {{\n    return {0};\n  }},\n", JsonConvert.SerializeObject(_component.Data, JsonSettings.JsonSerializerSettings));

            // render methods
            if (_component.Methods.Count > 0)
            {
                writer.Append("  methods: {\n");

                foreach (var m in _component.Methods.Values)
                {
                    writer.AppendFormat("    {0}: function({1}) {{{2}\n      this.$update(this, '{3}', [{1}]){4};\n    }}{5}\n",
                        m.Method.Name,
                        string.Join(", ", m.Parameters),
                        string.Join("\n      ", m.Pre),
                        m.Method.Name,
                        m.Post.Length > 0 ? "\n          .then(function(vm) { (function() {" + string.Join("\n      ", m.Post) + "\n          }).call(vm); })" : "",
                        m == _component.Methods.Last().Value ? "" : ",");
                }

                writer.Append("  },\n");
            }

            // render def
            if (_component.Computed.Count > 0)
            {
                writer.Append("  computed: {\n");

                foreach (var c in _component.Computed)
                {
                    writer.AppendFormat("    {0}: function() {{\n      {1};\n    }}{2}\n",
                        c.Key,
                        c.Value,
                        c.Key == _component.Computed.Last().Key ? "" : ",");
                }

                writer.Append("  },\n");
            }

            // render watchs
            if (_component.Watch.Count > 0)
            {
                writer.Append("  watch: {\n");

                foreach (var w in _component.Watch)
                {
                    writer.AppendFormat("    {0}: {{\n      handler: function(v, o) {{\n        if (this.$updating) return false;\n        this.{1}(v, o);\n      }},\n      deep: true\n    }}{2}\n",
                        w.Key, 
                        w.Value,
                        w.Key == _component.Watch.Last().Key ? "" : ",");
                }

                writer.Append("  },\n");
            }

            // render scripts
            if (_component.Scripts.Count > 0)
            {
                writer.Append("  mixins: [\n");

                foreach(var s in _component.Scripts)
                {
                    writer.AppendFormat("(function() {{\n{0}\n}})() || {{}}{1}\n",
                        s,
                        s == _component.Scripts.Last() ? "": ",");
                }

                writer.Append("  ],\n");
            }

            // render client-only properties
            writer.AppendFormat("  local: [{0}],\n", string.Join(", ", _component.Locals.Select(x => "'" + x + "'")));

            // add vpath to options
            writer.AppendFormat("  name: '{0}'\n", _component.Name);
            writer.Append(" }\n");

            writer.Append("};");
        }
    }
}

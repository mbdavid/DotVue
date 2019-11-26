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
    internal class ComponentRender
    {
        private readonly StringBuilder _writer;

        internal ComponentRender(StringBuilder writer)
        {
            _writer = writer;
        }

        public void RenderComponent(ComponentInfo component)
        {
            _writer.Append($"{{\n");

            // add vpath to options
            _writer.Append($"  name: '{component.Name}'\n");

            if (component.InheritAttrs == false)
            {
                _writer.Append($"  inheritAttrs: false,\n");
            }

            if (component.Props.Count > 0)
            {
                //_writer.AppendFormat("  props: [{0}],\n", string.Join(", ", _component.Props.Select(x => "'" + x.ToCamelCase() + "'")));
            }

            // append template string
            _writer.Append($"  template: `{component.Template.EncodeJavascript('`')}`,\n");
            _writer.Append($"  data: function() {{\n");
            _writer.Append($"      return {JsonConvert.SerializeObject(component.JsonData)};\n");
            _writer.Append($"  }}\n");

            // render methods
            if (component.Methods.Count > 0)
            {
                _writer.Append($"  methods: {{\n");

                foreach (var m in component.Methods.Values)
                {
                    var name = m.Method.Name.ToCamelCase();

                    _writer.Append($"    {name}: function(...args) {{\n");
                    _writer.Append($"      return this.$server('{name}', ...args);\n");
                    _writer.Append($"    }},\n"); // method
                }

                _writer.Append("  },\n"); // methods
            }

            // render watchs
            /*
            var watch = _component.Methods.Where(x => x.Value.Watch != null).ToArray();

            if (watch.Length > 0)
            {
                writer.Append("  watch: {\n");

                foreach (var w in watch)
                {
                    writer.AppendFormat("    {0}: {{\n      handler: function(v, o) {{\n        if (this.$updating) return false;\n        this.{1}(v, o);\n      }},\n      deep: true\n    }}{2}\n",
                        w.Value.Watch,
                        w.Value.Method.Name,
                        w.Key == watch.Last().Key ? "" : ",");
                }

                writer.Append("  },\n");
            }

            // render scripts
            if (_component.Scripts.Count > 0)
            {
                writer.Append("  mixins: [\n");

                foreach (var s in _component.Scripts)
                {
                    writer.AppendFormat("(function() {{\n{0}\n}})() || {{}}{1}\n",
                        s,
                        s == _component.Scripts.Last() ? "" : ",");
                }

                writer.Append("  ],\n");
            }
            */

            // register initial scripts and/or hood OnCreated server call
            if (component.CreatedHook || !string.IsNullOrEmpty(component.Scripts))
            {
                _writer.Append($"  created: function() {{\n");

                if (!string.IsNullOrEmpty(component.Scripts))
                {
                    _writer.Append(component.Scripts);
                }

                if (component.CreatedHook)
                {
                    _writer.Append($"    this.OnCreated();\n");
                }

                _writer.Append($"  }},\n");
            }

            _writer.Append("  mounted: function() { this.$emit('mounted'); }\n");

            _writer.Append($"}}");
        }
    }
}

﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            _writer.Append($"  name: '{component.Name}',\n");

            if (component.InheritAttrs == false)
            {
                _writer.Append($"  inheritAttrs: false,\n");
            }

            if (component.Props.Count > 0)
            {
                _writer.Append($"  props: {{\n");

                foreach(var prop in component.Props)
                {
                    var name = prop.Key.ToCamelCase();
                    var value = JsonConvert.SerializeObject(prop.Value);
                    var last = prop.Key == component.Props.Last().Key ? "" : ",";

                    _writer.Append($"    {name}: {{ default: {value} }}{last}\n");
                }

                _writer.Append($"  }},\n");
            }

            // append template string
            _writer.Append($"  template: `{component.Template.EncodeJavascript('`')}`,\n");
            _writer.Append($"  data: function() {{\n");
            _writer.Append($"      return {JsonConvert.SerializeObject(component.JsonData)};\n");
            _writer.Append($"  }},\n");

            // render methods
            if (component.Methods.Count > 0)
            {
                _writer.Append($"  methods: {{\n");

                foreach (var m in component.Methods)
                {
                    var name = m.Value.Method.Name.ToCamelCase();
                    var last = m.Key == component.Methods.Last().Key ? "" : ",";

                    _writer.Append($"    {name}: function(...args) {{\n");
                    _writer.Append($"      return this.$server('{name}', ...args);\n");
                    _writer.Append($"    }}{last}\n"); // method
                }

                _writer.Append("  },\n"); // methods
            }

            // render watchs
            var watch = component.Methods.Where(x => x.Value.Watch != null).ToArray();

            if (watch.Length > 0 || component.LocalStorage.Count > 0)
            {
                _writer.Append($"  watch: {{\n");

                foreach (var w in watch)
                {
                    var name = w.Value.Watch.ToCamelCase();
                    var method = w.Value.Method.Name.ToCamelCase();

                    _writer.Append($"    {name}: function(v, o) {{\n");
                    _writer.Append($"      this.{method}(v, o);\n");
                    _writer.Append($"    }},\n");
                }

                foreach (var w in component.LocalStorage)
                {
                    var name = w.Key.ToCamelCase();
                    var key = w.Value;

                    _writer.Append($"    {name}: function(v, o) {{\n");
                    _writer.Append($"      if(v === null || v === undefined) localStorage.removeItem('{key}'); else localStorage.setItem('{key}', v);\n");
                    _writer.Append($"    }},\n");
                }

                _writer.Append("  },\n");
            }

            // render mixin script
            if (component.Mixins.Count > 0)
            {
                _writer.Append($"  mixins: [\n");

                foreach (var s in component.Mixins)
                {
                    var last = s == component.Mixins.Last() ? "" : ",";

                    _writer.Append($"(function() {{\n");
                    _writer.Append(s);
                    _writer.Append($"}})() || {{}}{last}\n");
                }

                _writer.Append("  ],\n");
            }

            // declare server properties used. Client will exclude any local properties from postback
            IEnumerable<string> jsonDataProps = component.JsonData.Properties().Select(x => x.Name);
            IEnumerable<string> localProps = component.Locals.Select(x => x.Key.ToCamelCase());

             _writer.Append($"  serverProps: [{string.Join(", ", jsonDataProps.Except(localProps).Select(x => $"'{x}'"))}],\n");

            // register initial scripts and/or hood OnCreated server call
            if (component.CreatedHook || 
                !string.IsNullOrEmpty(component.Scripts) || 
                component.QueryString.Count > 0 || 
                component.RouteParams.Count > 0 || 
                component.LocalStorage.Count > 0)
            {
                _writer.Append($"  created: function() {{\n");

                foreach (var rp in component.RouteParams)
                {
                    var name = rp.Key.ToCamelCase();

                    _writer.Append($"    if (this.$route.params.{name} !== undefined) this.{name} = this.$route.params.{name};\n");
                }

                foreach (var qs in component.QueryString)
                {
                    var name = qs.Key.ToCamelCase();

                    _writer.Append($"    if (this.$route.query.{name} !== undefined) this.{name} = this.$route.query.{name};\n");
                }

                foreach (var ls in component.LocalStorage)
                {
                    var name = ls.Key.ToCamelCase();
                    var key = ls.Value;

                    _writer.Append($"    if (localStorage.getItem('{key}') !== null) this.{name} = localStorage.getItem('{key}');\n");
                }

                if (!string.IsNullOrEmpty(component.Scripts))
                {
                    _writer.Append(component.Scripts);
                }

                if (component.CreatedHook)
                {
                    _writer.Append($"    this.onCreated();\n");
                }

                _writer.Append($"  }},\n");
            }

            _writer.Append("  mounted: function() { this.$emit('mounted'); }\n");

            _writer.Append($"}}");
        }

        public void RenderRegister(IEnumerable<ComponentInfo> components)
        {
            foreach (var component in components)
            {
                _writer.Append($"Vue.component('{component.Name.ToCamelCase()}', {component.Name});\n");
            }
        }

        public void RenderRoutes(ICollection<ComponentInfo> components)
        {
            if (components.Count == 0) return;

            _writer.Append($"DotVue.routes = [\n");

            foreach(var component in components)
            {
                var last = component == components.Last() ? "" : ",";

                _writer.Append($"  {{ path: '{component.Route}', component: {component.Name} }}{last}\n");
            }

            _writer.Append($"];\n");
        }

        public void RenderStyles(IEnumerable<ComponentInfo> components)
        {
            var styles = new StringBuilder();

            foreach (var component in components.Where(x => !string.IsNullOrWhiteSpace(x.Styles)))
            {
                styles.Append(component.Styles);
            }

            if (styles.Length > 0)
            {
                try
                {
                    var s = dotless.Core.LessWeb.Parse(styles.ToString(), new dotless.Core.configuration.DotlessConfiguration
                    {
                        MinifyOutput = true,
                        CacheEnabled = false
                    });

                    _writer.Append($"var style = document.createElement('style');\n");
                    _writer.Append($"style.innerText = '{s.EncodeJavascript()}';\n");
                    _writer.Append($"document.head.appendChild(style);\n");
                }
                catch(Exception ex)
                {
                    _writer.Append($"document.body.innerHTML = '<h1 style=\"color:red\">LESS compiler error:{ex.Message.EncodeJavascript()}</h1><pre>{ex.StackTrace.EncodeJavascript()}</pre>'");
                }
            }
        }
    }
}

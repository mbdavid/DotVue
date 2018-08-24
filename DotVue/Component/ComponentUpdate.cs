using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    /// <summary>
    /// Execute requested method call and update viewmodel
    /// </summary>
    public class ComponentUpdate
    {
        private readonly ComponentInfo _component;
        private readonly IPrincipal _user;

        internal ComponentUpdate(ComponentInfo component, IPrincipal user)
        {
            _component = component;
            _user = user;
        }

        #region Update Models

        public async Task UpdateModel(ViewModel vm, string data, string props, string method, JToken[] parameters, IFormFileCollection files, TextWriter writer)
        {
            // populate my object with client $data
            JsonConvert.PopulateObject(data, vm, JsonSettings.JsonSerializerSettings);

            // populate my object with client $props
            JsonConvert.PopulateObject(props, vm, JsonSettings.JsonSerializerSettings);

            // parse $data as original value (before any update)
            var original = JObject.Parse(data);

            try
            {
                // set viewmodel request data
                ViewModel.SetData(vm, original);

                // if has method, call in existing vms
                this.ExecuteMethod(method, vm, parameters, files);

                // now, get viewmodel changes on data
                var current = JObject.FromObject(vm, JsonSettings.JsonSerializer);

                // merge all scripts
                var scripts = ViewModel.GetClientScript(vm);

                // detect changed from original to current data and send back to browser
                var diff = this.GetDiff(original, current);

                // write changes to writer
                using (var w = new JsonTextWriter(writer))
                {
                    var output = new JObject
                    {
                        { "update", diff },
                        { "script", scripts }
                    };
                    
                    await output.WriteToAsync(w);
                }
            }
            finally
            {
                // dispose vm
                vm.Dispose();
            }
        }

        /// <summary>
        /// Find a method in all componenets and execute if found
        /// </summary>
        private void ExecuteMethod(string name, ViewModel vm, JToken[] parameters, IFormFileCollection files)
        {
            var method = _component.Methods[name].Method;
            var pars = new List<object>();
            var index = 0;

            // convert each parameter as declared method in type
            foreach (var p in method.GetParameters())
            {
                var token = parameters[index++];

                if (p.ParameterType == typeof(IFormFile))
                {
                    var value = ((JValue)token).Value.ToString();

                    pars.Add(files.GetFile(value));
                }
                else if (p.ParameterType == typeof(IList<IFormFile>))
                {
                    var value = ((JValue)token).Value.ToString();
                
                    pars.Add(files.GetFiles(value));
                }
                else if (token.Type == JTokenType.Object)
                {
                    var obj = ((JObject)token).ToObject(p.ParameterType);

                    pars.Add(obj);
                }
                else if (token.Type == JTokenType.String && p.ParameterType.IsEnum)
                {
                    var value = ((JValue)token).Value.ToString();

                    pars.Add(Enum.Parse(p.ParameterType, value));
                }
                else
                {
                    var value = ((JValue)token).Value;

                    pars.Add(Convert.ChangeType(value, p.ParameterType));
                }
            }

            // now execute method inside viewmodel
            ViewModel.Execute(vm, method, pars.ToArray());
        }

        /// <summary>
        /// Create a new object with only diff between original viewmodel and new changed viewmodel
        /// </summary>
        private JObject GetDiff(JObject original, JObject current)
        {
            // create a diff object to capture any change from original to current data
            var diff = new JObject();

            foreach (var item in current)
            {
                var orig = original[item.Key];

                if (orig == null && item.Value.HasValues == false) continue;

                // use a custom compare function
                if (JTokenComparer.Instance.Compare(orig, item.Value) != 0)
                {
                    diff[item.Key] = item.Value;
                }
            }

            return diff;
        }

        #endregion
    }
}

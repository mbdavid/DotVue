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
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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

        public async Task UpdateModel(ViewModel vm, string data, string props, string method, JToken[] parameters, HttpContext context, TextWriter writer)
        {
            var jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ContractResolver = new CustomContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            // populate my object with client $data
            JsonConvert.PopulateObject(data, vm);

            // populate my object with client $props
            JsonConvert.PopulateObject(props, vm);

            // populate all cookies attributes
            this.ReadCookies(vm, context.Request.Cookies);

            // parse $data as original value (before any update)
            var original = JObject.FromObject(vm, jsonSerializer);

            try
            {
                // set viewmodel request data
                ViewModel.SetData(vm, original);

                // if has method, call in existing vms
                var result = this.ExecuteMethod(method, vm, parameters, context.Request.Form.Files, context.RequestServices);

                // now, get viewmodel changes on data
                var current = JObject.FromObject(vm, jsonSerializer);

                // merge all scripts
                var scripts = ViewModel.GetClientScript(vm);

                // detect changed from original to current data and send back to browser
                var diff = this.GetDiff(original, current);

                // write cookies into http response
                this.WriteCookies(vm, context.Response.Cookies);

                // write changes to writer
                using (var w = new JsonTextWriter(writer))
                {
                    var output = new JObject
                    {
                        { "update", diff },
                        { "script", scripts },
                        { "result", result == null ? null : JToken.FromObject(result) }
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
        private object ExecuteMethod(string name, ViewModel vm, JToken[] parameters, IFormFileCollection files, IServiceProvider serviceProvider)
        {
            var met = _component.Methods[name];
            var method = met.Method;
            var pars = new List<object>();
            var index = 0;

            // check for permissions
            if (met.IsAuthenticated && _user.Identity.IsAuthenticated == false) throw new HttpException(401);
            if (met.Roles.Length > 0 && met.Roles.Any(x => _user.IsInRole(x)) == false) throw new HttpException(403, $"Forbidden. This method requires all this roles: `{string.Join("`, `", met.Roles)}`");

            // convert each parameter as declared method in type
            foreach (var p in method.GetParameters())
            {
                // if has no passed parameter, try create instance based on DI
                if (index >= parameters.Length)
                {
                    var value = serviceProvider.GetService(p.ParameterType);

                    pars.Add(value);

                    continue;
                }

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
            return ViewModel.Execute(vm, method, pars.ToArray());
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

        private void ReadCookies(ViewModel vm, IRequestCookieCollection cookies)
        {
            var type = vm.GetType();

            foreach (var field in _component.Cookies)
            {
                var value = cookies[field.Key];

                if (value != null)
                {
                    field.Value.SetValue(vm, Convert.ChangeType(value, field.Value.FieldType));
                }
            }
        }

        private void WriteCookies(ViewModel vm, IResponseCookies cookies)
        {
            var type = vm.GetType();

            foreach (var field in _component.Cookies)
            {
                var value = field.Value.GetValue(vm);

                if (value != null)
                {
                    cookies.Append(field.Key, value.ToString());
                }
                else
                {
                    cookies.Delete(field.Key);
                }
            }
        }

        #endregion
    }
}

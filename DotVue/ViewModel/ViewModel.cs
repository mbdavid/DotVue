using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace DotVue
{
    public class ViewModel : IDisposable
    {
        /// <summary>
        /// Get instance of Javascript builder to be run after update/create vue instance
        /// </summary>
        protected JavascriptBuilder ClientScript { get; private set; } = new JavascriptBuilder();

        /// <summary>
        /// Get requested viewmodel data from client (before any change)
        /// </summary>
        protected JObject Data { get; private set; } = new JObject();

        /// <summary>
        /// In page call during initialize. In component, made ajax call when component are created
        /// </summary>
        protected virtual void OnCreated()
        {
        }

        /// <summary>
        /// Override this method to capture execute method from client
        /// </summary>
        protected virtual object OnExecute(MethodInfo method, object[] parameters)
        {
            return method.Invoke(this, parameters);
        }

        public virtual void Dispose()
        {
        }

        #region Static caller

        internal static void CallOnCreated(ViewModel vm)
        {
            vm.OnCreated();
        }

        internal static void SetData(ViewModel vm, JObject data)
        {
            vm.Data = data;
        }

        internal static string GetClientScript(ViewModel vm)
        {
            return vm.ClientScript.ToString();
        }

        internal static object Execute(ViewModel vm, MethodInfo method, object[] parameters)
        {
            return vm.OnExecute(method, parameters);
        }

        #endregion
    }
}

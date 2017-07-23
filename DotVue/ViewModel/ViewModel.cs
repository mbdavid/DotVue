using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    public class ViewModel : IDisposable
    {
        private JavascriptBuilder _script = new JavascriptBuilder();
        private JObject _data = null;

        /// <summary>
        /// Get instance of Javascript builder to be run after update/create vue instance
        /// </summary>
        protected JavascriptBuilder ClientScript { get { return _script; } }

        /// <summary>
        /// Get request data from client
        /// </summary>
        protected JObject Data { get { return _data; } }

        /// <summary>
        /// In page call during initialize. In component, made ajax call when component are created
        /// </summary>
        protected virtual void OnCreated()
        {
        }

        /// <summary>
        /// Override this method to capture execute method from client
        /// </summary>
        protected virtual void OnExecute(MethodInfo method, object[] parameters)
        {
            method.Invoke(this, parameters);
        }

        #region Computed

        /// <summary>
        /// Resolve an expression to convert into a computed field
        /// </summary>
        public static Computed Resolve<T>(Expression<Func<T, object>> expr) where T : ViewModel
        {
            return new Computed
            {
                Code = ExprJs.Resolve(expr),
                Value = (object o) => expr.Compile()
            };
        }

        #endregion

        public virtual void Dispose()
        {
        }

        #region Static caller

        internal static void Created(ViewModel vm)
        {
            vm.OnCreated();
        }

        internal static string Script(ViewModel vm)
        {
            return vm._script.ToString();
        }

        internal static void Execute(ViewModel vm, MethodInfo method, object[] parameters)
        {
            vm.OnExecute(method, parameters);
        }

        internal static void SetData(ViewModel vm, JObject data)
        {
            vm._data = data;
        }

        #endregion
    }
}

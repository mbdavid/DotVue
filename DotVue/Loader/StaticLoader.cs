//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Web;

//namespace DotVue
//{
//    /// <summary>
//    /// Implement component loader based static type model and stream content
//    /// </summary>
//    public class StaticLoader : IComponentLoader
//    {
//        private static Dictionary<string, Component> _components = new Dictionary<string, Component>(StringComparer.OrdinalIgnoreCase);

//        public static void RegisterComponent(string name, string content, Type viewModelType = null)
//        {
//            _components[name] = new Component(
//                "/_/" + name + ".vue",
//                viewModelType ?? typeof(EmptyViewModel),
//                content
//            );
//        }

//        public IEnumerable<ComponentInfo> Discover(HttpContext context)
//        {
//            foreach(var key in _components.Keys)
//            {
//                yield return new ComponentInfo
//                {
//                    Name = key,
//                    VPath = "/_/" + key + ".vue"
//                };
//            }
//        }

//        public Component Load(HttpContext context, string vpath)
//        {
//            var name = Path.GetFileNameWithoutExtension(vpath);
//            Component c;

//            if(_components.TryGetValue(name, out c))
//            {
//                return c;
//            }

//            return null;
//        }
//    }
//}

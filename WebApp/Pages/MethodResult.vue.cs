using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class MethodResult : ViewModel
    {
        public MethodResult()
        {
        }

        public string[] GetList(int qt)
        {
            return new string[] { "Item 1", "Item " + qt };
        }
    }
}
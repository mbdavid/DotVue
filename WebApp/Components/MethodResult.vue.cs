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

        [Confirm("Confirm execute?")]
        [Script(null, "alert('postScript: ' + this.$refs.btn.innerText)")]
        [Script(null, "alert('postScript2')")]
        public string[] GetList(int qt)
        {
            return new string[] { "Item 1", "Item " + qt };
        }
    }
}
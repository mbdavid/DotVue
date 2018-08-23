using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class Error : ViewModel
    {
        public string Id { get; set; } = "My first id";
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Ok { get; set; }

        public void ClickError(int a, int b, string c)
        {
            var zero = 0;
            var i = 1 / zero;
        }
    }
}
using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class LayoutContent : ViewModel
    {
        public bool IsSaveEnabled { get; set; } = true;

        public void Save()
        {
            this.ClientScript.Alert("Save inside content");
        }
    }
}
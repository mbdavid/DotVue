using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class FormInner : ViewModel
    {
        public string Id { get; set; }

        public void SaveClick()
        {
            this.ClientScript.Emit("onsave", this.Id);
        }
    }
}
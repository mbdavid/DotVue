using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class FormEmit : ViewModel
    {
        public void Save(string id)
        {
            this.ClientScript.Alert("Saved from InnerForm with id = " + id);
        }
    }
}
using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class SimpleLayout : ViewModel
    {
        [Prop]
        public bool SaveEnabled { get; set; }

        public void LocalTest()
        {
            ClientScript.Alert("Is SaveEnabled? = " + SaveEnabled);
        }
    }
}
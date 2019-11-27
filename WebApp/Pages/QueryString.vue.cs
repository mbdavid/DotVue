using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages
{
    public class QueryString : ViewModel
    {
        [QueryString]
        public string Id;

        [QueryString]
        public string R;

        public int Counter { get; set; } = 1;

        public void Id_Watch()
        {
            this.ClientScript.ConsoleLog("Watch on query from QueryString");
            this.OnCreated();
        }

        public void ShowQuery()
        {
            this.ClientScript.Alert("ID = " + Id);
        }

        public void Inc()
        {
            this.Counter++;
        }
    }
}
using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages
{
    public class Route : ViewModel
    {
        [RouteParam]
        public string Id;

        [QueryString]
        public string Page;

        public void ShowData()
        {
            this.ClientScript.Alert("ID = " + Id + " - Page = " + this.Page);
        }
    }
}
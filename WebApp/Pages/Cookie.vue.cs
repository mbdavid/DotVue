using DotVue;
using System;

namespace WebApp.Pages
{
    public class Cookie : ViewModel
    {
        [Cookie]
        public string MyCookie = "init";

        public string Value { get; set; }

        public void ReadCookie()
        {
            ClientScript.Alert(this.MyCookie);
        }

        public void UpdateCookie()
        {
            this.MyCookie = this.Value;
        }
    }
}

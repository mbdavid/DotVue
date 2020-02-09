using DotVue;
using System;

namespace WebApp.Pages
{
    public class Index : ViewModel
    {
        public string Text { get; set; } = "Initial value";

        [Prop]
        public string Code = "ok";

        public void ShowCode()
        {
            ClientScript.Alert(this.Code);
        }

        //public void Text_Watch(string newValue, string oldValue)
        //{
        //}
    }
}

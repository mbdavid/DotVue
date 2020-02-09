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
            ClientScript.Code("this.$router.push('todo-client');");
        }

        //public void Text_Watch(string newValue, string oldValue)
        //{
        //}
    }
}

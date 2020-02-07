using DotVue;
using System;

namespace WebApp.Pages
{
    public class Index : ViewModel
    {
        public string Text { get; set; } = "Initial value";

        public int Id { get; set; } = 15;

        [Prop]
        public string Code = "ok";

        protected override void OnCreated()
        {
            this.Id = 100;
        }

        //[Script("this.$on('{name}:before', function(p) { });")]
        public int ClickMe(int step)
        {
            this.Id += step;

            return this.Id;
        }

        public void ShowCode()
        {
            ClientScript.Code("this.$router.push('todo-client');");
        }

        //public void Text_Watch(string newValue, string oldValue)
        //{
        //}
    }
}

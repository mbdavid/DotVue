using DotVue;
using System;

namespace WebApp.Pages
{
    public class Index : ViewModel
    {
        public string Text { get; set; }

        public int Id { get; set; } = 15;

        [Prop]
        public string Codigo = "ok";

        protected override void OnCreated()
        {
            this.Id = 100;
        }

        //[Script("this.$on('{name}:before', function(p) { });")]
        public void ClickMe(int step)
        {
            this.Id += step;
        }
    }
}

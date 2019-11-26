using DotVue;
using System;

namespace WebApp.Pages
{
    public class Index : ViewModel
    {
        public string Text { get; set; }

        public int Id { get; set; } = 15;

        protected override void OnCreated()
        {
            this.Id = 100;
        }

        public void ClickMe(int step)
        {
            this.Id += step;
        }
    }
}

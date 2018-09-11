using DotVue;
using System;

namespace WebApp.Components
{
    public class Page1 : ViewModel
    {
        public string Text { get; set; }

        public int Id { get; set; } = 15;

        [Watch("Text")]
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

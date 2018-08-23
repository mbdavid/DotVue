using DotVue;
using System;

namespace WebApp.Components
{
    public class Page1 : ViewModel
    {
        public int Id { get; set; } = 15;

        public void ClickMe(int step)
        {
            this.Id += step;
        }
    }
}

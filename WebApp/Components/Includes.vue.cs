using DotVue;
using System;

namespace WebApp.Components
{
    public class Includes : ViewModel
    {
        public int Id { get; set; } = 15;

        public void ClickMe(int step)
        {
            this.Id += step;
        }
    }
}

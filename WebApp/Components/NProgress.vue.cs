using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class NProgress : ViewModel
    {
        public string Done { get; set; }

        [NProgress, Ready("hidden")]
        protected override void OnCreated()
        {
            System.Threading.Thread.Sleep(3000);

            this.Done = "Document done from server: " + DateTime.Now;
        }

        [NProgress, Loading("btn")]
        public void Wait(int time)
        {
            System.Threading.Thread.Sleep(time);
        }
    }
}
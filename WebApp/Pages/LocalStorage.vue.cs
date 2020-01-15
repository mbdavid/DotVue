using DotVue;
using System;

namespace WebApp.Pages
{
    public class LocalStorage : ViewModel
    {
        [LocalStorage("myKey")]
        public string MyData { get; set; } = "my init data";

        public void UpdateData()
        {
            this.MyData = DateTime.Now.ToString();
        }
    }
}

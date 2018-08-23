using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class Local : ViewModel
    {
        [Local]
        public List<string> Items { get; set; } = null;

        public Local()
        {
            this.Items = new List<string> { "Constructor", "Values" };
        }

        protected override void OnCreated()
        {
            this.Items = new List<string> { "OnCreated", "Values" };
        }

        public void LoadData()
        {
            this.Items = new List<string> { "New", "Load", "Data" };
        }

        public void ChangeData()
        {
            this.Items = new List<string> { "Changed", "Data" };
        }

        public void CheckData()
        {
            this.ClientScript.Alert("Current Items in Server: " + string.Join(", ", Items));
        }
    }
}
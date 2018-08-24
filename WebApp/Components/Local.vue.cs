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
        }

        protected override void OnCreated()
        {
            // local properties are only from Server-to-Client
            this.Items = new List<string> { "Initial", "Values", "From", "Ctor", "ViewModel", DateTime.Now.ToString() };
        }

        public void ChangeData()
        {
            this.Items = new List<string> { "Changed" };
        }

        public void CheckData()
        {
            this.ClientScript.Alert("Items in server must be null (because are initialized only on OnCreated()): " + (Items == null));
        }
    }
}
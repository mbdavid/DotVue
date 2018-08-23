using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class QueryString : ViewModel
    {
        [Prop]
        public Dictionary<string, string> Query { get; set; } = new Dictionary<string, string>();

        public string Id { get; set; }

        public int Counter { get; set; } = 1;

        protected override void OnCreated()
        {
            this.Id = this.Query.GetValueOrDefault("id");
        }

        public void query_Watch()
        {
            this.ClientScript.ConsoleLog("Watch on query from Page2");
            this.OnCreated();
        }

        public void ShowQuery()
        {
            this.ClientScript.Alert("ID = " + Id);
        }

        public void Inc()
        {
            this.Counter++;
        }
    }
}
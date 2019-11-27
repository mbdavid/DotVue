using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class TodoClient : ViewModel
    {
        public string CurrentText { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        public void Save()
        {
            this.ClientScript.Alert($"From Server - Saved {this.Items.Count} items");
        }

        public class Todo
        {
            public string Text { get; set; }
            public bool Done { get; set; }
        }
    }
}
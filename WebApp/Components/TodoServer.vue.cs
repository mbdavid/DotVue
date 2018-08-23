using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class TodoServer : ViewModel
    {
        public string CurrentText { get; set; } = "";
        public string FilterText { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        [Computed("x", "x.Items.filter(v => v.Text.toUpperCase().indexOf(x.FilterText.toUpperCase()) >= 0)")]
        public IEnumerable<Todo> Filtered => this.Items.Where(z => z.Text.ToUpper().Contains(this.FilterText.ToUpper()));

        protected override void OnCreated()
        {
            this.Items.Add(new Todo { Text = "My first demo" });
            this.Items.Add(new Todo { Text = "Was done", Done = true });
        }

        public void Add()
        {
            //this.Items.Add(new Todo { Text = CurrentText + " (filtered: " + this.Filtered.Value.Count() + ")", Done = false });
            this.Items.Add(new Todo { Text = CurrentText + " (filtered: " + this.Filtered.Count() + ")", Done = false });
            this.CurrentText = "";
            this.ClientScript.Focus("input");
        }

        public void CurrentText_Watch(string newValue)
        {
            if (newValue == "a") CurrentText = "AAA";
        }

        public void Remove(int index)
        {
            this.Items.RemoveAt(index);
        }

        [Confirm("Clear all items?")]
        public void Clear()
        {
            this.Items.Clear();
        }
    }

    public class Todo
    {
        public string Text { get; set; }
        public bool Done { get; set; }
    }
}
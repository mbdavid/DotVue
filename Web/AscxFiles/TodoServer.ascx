<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
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

</script>
<template>
    <div>
        <h3>Todo List - Server Only</h3>
        <hr />
        <div>
            <input type="text" v-model="CurrentText" @keyup.enter="Add()" autofocus placeholder="Add new Item" ref="input"/>
            <button @click="Add()" :disabled="!CurrentText">Add</button>
            <button @click="Clear()">Clear all</button>
            <input type="text" v-model="FilterText" placeholder="Filter by text" />
        </div>
        <hr />
        <ul>
            <li v-for="(Item, i) in Filtered">
                <input type="checkbox" v-model="Item.Done" />
                <span :style="{ 'text-decoration': Item.Done ? 'line-through' : 'none' }" @click="Item.Done = !Item.Done">
                    {{ Item.Text }}
                </span>
                <button v-on:click.prevent="Remove(i)" :disabled="Item.Done" type="submit">X</button>
            </li>
        </ul>
        <i v-show="Filtered.length != Items.length">
            Showing only {{ Filtered.length }} from {{ Items.length }} items
        </i>
    </div>
</template>
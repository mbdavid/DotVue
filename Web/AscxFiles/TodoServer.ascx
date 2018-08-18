<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string CurrentText { get; set; } = "";
        public string FilterText { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        [Computed("x", "x.items.filter(v => v.text.toUpperCase().indexOf(x.filterText.toUpperCase()) >= 0)")]
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

</script>
<template>
    <div>
        <h3>Todo List - Server Only</h3>
        <hr />
        <div>
            <input type="text" v-model="currentText" @keyup.enter="add()" autofocus placeholder="Add new Item" ref="input"/>
            <button @click="add()" :disabled="!currentText">Add</button>
            <button @click="clear()">Clear all</button>
            <input type="text" v-model="filterText" placeholder="Filter by text" />
        </div>
        <hr />
        <ul>
            <li v-for="(item, i) in filtered">
                <input type="checkbox" v-model="item.done" />
                <span :style="{ 'text-decoration': item.done ? 'line-through' : 'none' }" @click="item.done = !item.done">
                    {{ item.text }}
                </span>
                <button v-on:click.prevent="remove(i)" :disabled="item.Done" v-show="filterText.length == 0" type="submit">X</button>
            </li>
        </ul>
        <i v-show="filtered.length != items.length">
            Showing only {{ filtered.length }} from {{ items.length }} items
        </i>
    </div>
</template>
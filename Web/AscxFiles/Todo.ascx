<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string CurrentText { get; set; } = "";
        public string FilterText { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        public Computed Filtered = Resolve<ComponentVM>(x => x.Items.Where(z => z.Text.Contains(x.FilterText)));

        protected override void OnCreated()
        {
            Items.Add(new Todo { Text = "My first demo" });
            Items.Add(new Todo { Text = "Was done", Done = true });
        }

        public void Add()
        {
            Items.Add(new Todo { Text = CurrentText, Done = false });
            CurrentText = "";
        }

        public void Remove(int index)
        {
            Items.RemoveAt(index);
        }

        [Confirm("Clear all items?")]
        public void Clear()
        {
            Items.Clear();
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
        <h3>Todo List</h3>
        <hr />
        <div>
            <input type="text" v-model="CurrentText" autofocus placeholder="Add new Item"/>
            <button v-on:click="Add()" :disabled="!CurrentText" type="button">Add</button>
            <button v-on:click="Clear()" type="button">Clear all</button>
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
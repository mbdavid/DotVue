<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string CurrentText { get; set; } = "";
        public string FilterText { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        public void Save()
        {
            this.ClientScript.Alert($"From Server - Saved {this.Items.Count} items");
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
        <h3>Todo List - Client Only - Server only on Save()</h3>
        <hr />
        <div>
            <input type="text" v-model="currentText" @keyup.enter="add()" autofocus placeholder="Add new Item" ref="input"/>
            <button @click="add()" :disabled="!currentText">Add</button>
            <button @click="clear()">Clear all</button>
        </div>
        <hr />
        <ul>
            <li v-for="(item, i) in items">
                <input type="checkbox" v-model="item.done" />
                <span :style="{ 'text-decoration': item.done ? 'line-through' : 'none' }" @click="item.done = !item.done">
                    {{ item.text }}
                </span>
                <button v-on:click.prevent="remove(i)" :disabled="item.done" type="submit">X</button>
            </li>
        </ul>
        <hr />
        <button type="button" @click="save">Save on Server</button>
    </div>
</template>

<script>
    
    return {
        created: function () {
            this.items.push({ text: "My first demo", done: false });
            this.items.push({ text: "Was done", done: true });
        },
        methods: {
            add() {
                this.items.push({ text: this.currentText, done: false });
                this.currentText = "";
                this.$refs.input.focus();
            },
            remove(index) {
                this.items.splice(index, 1);
            },
            clear() {
                this.items.splice(0, this.items.length);
            }
        }
    };

</script>
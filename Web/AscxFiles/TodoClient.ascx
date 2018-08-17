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
            <input type="text" v-model="CurrentText" @keyup.enter="Add()" autofocus placeholder="Add new Item" ref="input"/>
            <button @click="Add()" :disabled="!CurrentText">Add</button>
            <button @click="Clear()">Clear all</button>
        </div>
        <hr />
        <ul>
            <li v-for="(Item, i) in Items">
                <input type="checkbox" v-model="Item.Done" />
                <span :style="{ 'text-decoration': Item.Done ? 'line-through' : 'none' }" @click="Item.Done = !Item.Done">
                    {{ Item.Text }}
                </span>
                <button v-on:click.prevent="Remove(i)" :disabled="Item.Done" type="submit">X</button>
            </li>
        </ul>
        <hr />
        <button type="button" @click="Save">Save on Server</button>
    </div>
</template>

<script>
    
    return {
        created: function () {
            this.Items.push({ Text: "My first demo", Done: false });
            this.Items.push({ Text: "Was done", Done: true });
        },
        methods: {
            Add() {
                this.Items.push({ Text: this.CurrentText, Done: false });
                this.CurrentText = "";
                this.$refs.input.focus();
            },
            Remove(index) {
                this.Items.splice(index, 1);
            },
            Clear() {
                this.Items.splice(0, this.Items.length);
            }
        }
    };

</script>
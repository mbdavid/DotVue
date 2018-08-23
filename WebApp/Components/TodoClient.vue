@viewmodel WebApp.Components.TodoClient

<template>
    <div>
        <h3>Todo List - Client Only - Server only on Save()</h3>
        <hr />
        <div>
            <input type="text" v-model="CurrentText" @keyup.enter="Add()" autofocus placeholder="Add new Item" ref="input" />
            <button @click="Add()" :disabled="!CurrentText">Add</button>
            <button @click="Clear()">Clear all</button>
        </div>
        <hr />
        <ul>
            <li v-for="(item, i) in Items">
                <input type="checkbox" v-model="item.Done" />
                <span :style="{ 'text-decoration': item.Done ? 'line-through' : 'none' }" @click="item.Done = !item.Done">
                    {{ item.Text }}
                </span>
                <button v-on:click.prevent="Remove(i)" :disabled="item.Done" type="submit">X</button>
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
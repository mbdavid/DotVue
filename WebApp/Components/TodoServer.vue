@viewmodel WebApp.Components.TodoServer

<template>
    <div>
        <h3>Todo List - Server Only</h3>
        <hr />
        <div>
            <input type="text" v-model="CurrentText" @keyup.enter="Add()" autofocus placeholder="Add new Item" ref="input" />
            <button @click="Add()" :disabled="!CurrentText">Add</button>
            <button @click="Clear()">Clear all</button>
            <input type="text" v-model="FilterText" placeholder="Filter by text" />
        </div>
        <hr />
        <ul>
            <li v-for="(item, i) in Filtered">
                <input type="checkbox" v-model="item.Done" />
                <span :style="{ 'text-decoration': item.Done ? 'line-through' : 'none' }" @click="item.Done = !item.Done">
                    {{ item.Text }}
                </span>
                <button v-on:click.prevent="Remove(i)" :disabled="item.Done" v-show="FilterText.length == 0" type="submit">X</button>
            </li>
        </ul>
        <i v-show="Filtered.length != Items.length">
            Showing only {{ Filtered.length }} from {{ Items.length }} items
        </i>
    </div>
</template>
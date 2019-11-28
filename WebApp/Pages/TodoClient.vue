@page /todo-client

<div>
    <h3>Todo List - Client Only - Server only on Save()</h3>
    <hr />
    <div>
        <input type="text" v-model="currentText" @keyup.enter="add()" autofocus placeholder="Add new Item" ref="input" />
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
            <button v-on:click.prevent="Remove(i)" :disabled="item.done" type="submit">X</button>
        </li>
    </ul>
    <hr />
    <button type="button" @click="save()">Save on Server</button>
    <pre>{{$data}}</pre>
</div>

<script>

    this.items.push({ text: "My first demo", done: false });
    this.items.push({ text: "Was done", done: true });

    this.add = function () {
        this.items.push({ text: this.currentText, done: false });
        this.currentText = "";
        this.$refs.input.focus();
    }

    this.remove = function () {
        this.items.splice(index, 1);
    }

    this.clear = function () {
        this.items.splice(0, this.Items.length);
    }

</script>
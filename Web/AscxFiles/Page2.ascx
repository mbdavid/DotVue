<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        [Prop]
        public Dictionary<string, string> query { get; set; } = new Dictionary<string, string>();

        public string Id { get; set; }

        public int Counter { get; set; } = 1;

        protected override void OnCreated()
        {
            Id = query.GetValueOrDefault("id");
        }

        public void query_Watch()
        {
            ClientScript.ConsoleLog("Watch on query from Page2");
            OnCreated();
        }

        public void ShowQuery()
        {
            ClientScript.Alert("ID = " + Id);
        }

        public void Inc()
        {
            Counter++;
        }
    }

</script>
<template>
    <div class="page2">
        <h4>Page 2</h4><hr />
        <div>
            [<a href="#/Page2">Empty QueryString</a>]
            [<a href="#/Page2?id=123">?id=123</a>]
            [<a href="#/Page2?id=ABC&r=4455">?id=ABC&r=4455</a>]
        </div>

        <p>Content from page2, content from page2, Content from page2</p>
        <p>Content from page2, content from page2, Content from page2</p>
        <button @click="ShowQuery()">ShowQuery()</button>
        <button @click="Inc()">{{ Counter }}</button>
        <pre>$data: {{ $data }}</pre>
        <pre>$props: {{ $props }}</pre>
    </div>
</template>
<style>
    .page2 h4 { color: blue; }
</style>
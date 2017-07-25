<%@ Control Language="C#" %>
<script runat="server">

    public class LocalExample : ViewModel
    {
        [Local]
        public List<string> Items { get; set; } = null;

        public LocalExample()
        {
            this.Items = new List<string> { "Constructor", "Values" };
        }

        protected override void OnCreated()
        {
            this.Items = new List<string> { "OnCreated", "Values" };
        }

        public void LoadData()
        {
            Items = new List<string> { "New", "Load", "Data" };
        }

        public void ChangeData()
        {
            Items = new List<string> { "Changed", "Data" };
        }

        public void CheckData()
        {
            ClientScript.Alert("Current Items in Server: " + string.Join(", ", Items));
        }
    }

</script>
<template>
    <div>
        <button @click="LoadData()">LoadData()</button>
        <button @click="ChangeData()">ChangeData()</button>
        <button @click="CheckData()">CheckData()</button>
        <hr />
        <ul><li v-for="i in Items">{{ i }}</li></ul>
        <hr />
        <pre>{{ $data }}</pre>
    </div>
</template>
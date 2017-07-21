<%@ Control Language="C#" %>
<script runat="server">

    public class Agenda : ViewModel
    {
        [Local]
        public List<string> Items { get; set; } = null;
        public string Title { get; set; }

        public void LoadData()
        {
            Items = new List<string> { "a", "b", "c" };
            Title = "This is an title";
        }

        public void ChangeData()
        {
            Items = new List<string> { "x", "y", "z" };
        }

        public void CheckData()
        {
            ClientScript.Alert("Is Items == null? " + (Items == null) + "\n" +
                "Is Title == null? " + (Title == null));
        }
    }

</script>
<template>
    <div>
        <button @click="LoadData()">LoadData()</button>
        <button @click="ChangeData()">ChangeData()</button>
        <button @click="CheckData()">CheckData()</button>
        <hr />
        <h3>Title: {{ Title }}</h3>
        <ul><li v-for="i in Items">{{ i }}</li></ul>
        <hr />
        <pre>{{ $data }}</pre>
    </div>
</template>
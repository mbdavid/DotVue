<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string Id { get; set; }

        public void SaveClick()
        {
            this.ClientScript.Emit("onsave", this.Id);
        }
    }

</script>
<template>
    <div style="border: 1px solid blue; padding: 20px;">
        <h3>Inner Form</h3>
        <hr />
        <input type="text" v-model="Id" placeholder="Id value" />

        <button type="button" @click="SaveClick()">Save on Server</button>

    </div>

</template>

<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public bool IsSaveEnabled { get; set; } = true;

        public void Save()
        {
            this.ClientScript.Alert("Save inside content");
        }
    }

</script>
<template>
    <simple-layout @save="save()" :save-enabled="isSaveEnabled">

        <template slot="toolbar">
            <button @click="isSaveEnabled = !isSaveEnabled">Toggle</button>
        </template>

        <template slot="form">
            This your content inside <b>simple-layout</b><br />
            Save options is: {{ IsSaveEnabled }}
        </template>

    </simple-layout>

</template>

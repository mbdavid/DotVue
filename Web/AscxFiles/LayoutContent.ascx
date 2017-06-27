<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public bool IsSaveEnabled { get; set; } = true;

        public void Save()
        {
            JS.Alert("Save inside content");
        }
    }

</script>
<template>
    <simple-layout @save="Save()" :save-enabled="IsSaveEnabled">

        <template slot="toolbar">
            <button @click="IsSaveEnabled = !IsSaveEnabled">Toggle</button>
        </template>

        <template slot="form">
            This your content inside <b>simple-layout</b><br />
            Save options is: {{ IsSaveEnabled }}
        </template>

    </simple-layout>

</template>
<style>
    .base-layout { border: 1px solid red; }
</style>
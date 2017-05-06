<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public int Counter { get; set; }
    }

</script>
<template>
    <div class="page1">
        <h3>Page 1</h3><hr />
        <p>Content from page1</p>
        <button @click="Counter++">{{ Counter }}</button>

    </div>
</template>
<style>
    .page1 h3 { color: red; }
</style>
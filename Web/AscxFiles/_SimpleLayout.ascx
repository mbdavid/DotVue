<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        [Prop]
        public bool SaveEnabled { get; set; }
    }

</script>
<template>
    <div class="simple-layout">
        <h1>Simple Layout</h1>
        <hr />
        <div class="toolbar">
            <button @click="$emit('save')" :disabled="!SaveEnabled">Save</button>
            <slot name="toolbar"></slot>
        </div>
        <hr />
        <form>
            <slot name="form"></slot>
        </form>
    </div>
</template>
<style>
    .simple-layout { border: 2px solid red; margin: 20px; }
    .simple-layout .toolbar { margin: 10px; border: 1px solid blue; padding: 10px; } 
    .simple-layout form { margin: 10px; border: 1px solid blue; padding: 10px; } 
</style>
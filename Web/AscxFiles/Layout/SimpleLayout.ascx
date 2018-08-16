<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        [Prop]
        public bool SaveEnabled { get; set; }

        public void LocalTest()
        {
            ClientScript.Alert("Is SaveEnabled? = " + SaveEnabled);
        }
    }

</script>
<template>
    <div class="simple-layout">
        <h1>Simple Layout</h1>
        <hr />
        <div class="toolbar">
            <button @click="$emit('save')" :disabled="!SaveEnabled">Save</button>
            <button @click="LocalTest()">Test if save are enabled</button>
            <span class="slot">
                <slot name="toolbar"></slot>
            </span>
        </div>
        <hr />
        <form class="slot">
            <slot name="form"></slot>
        </form>
    </div>
</template>
<style>
    @import url("Assets/Styles/base.css");

    .simple-layout { border: 2px solid red; margin: 20px; padding: 20px; background-color: #ffbfbf; }
    .simple-layout .slot { padding: 10px; background-color:var(--main-bg-color); border: 1px solid red; } 
</style>
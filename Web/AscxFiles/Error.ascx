<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public void ClickError()
        {
            var zero = 0;
            var i = 1 / zero;
        }
    }

</script>
<template>
    <div>
        <button @click="ClickError()">Error in server</button>


    </div>
</template>
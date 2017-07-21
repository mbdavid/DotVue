<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public void Error1()
        {
            var zero = 0;
            var i = 1 / zero;
        }
    }

</script>
<template>
    <div>
        <button @click="Error1()">Error in server</button>


    </div>
</template>
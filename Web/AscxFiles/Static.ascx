<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public int Counter { get; set; }
    }

</script>
<template>
    <div class="page-static">

        <h3>Static Loader</h3><hr />

        <red-button>RedButton</red-button>

    </div>
</template>

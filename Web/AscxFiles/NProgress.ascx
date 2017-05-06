<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string Done { get; set; }

        [NProgress, Ready("hidden")]
        protected override void OnCreated()
        {
            System.Threading.Thread.Sleep(3000);

            Done = "Document done from server: " + DateTime.Now;
        }
    }

</script>
<template>
    <div class="hidden">
        <h3>Progress bar</h3><hr />
        {{ Done }}
        <hr />
        <button @click="finish()">Destroy and Go to Page1</button>
    </div>
</template>
<script>
    return {
        methods: {
            finish: function() {
                this.$destroy();
                location.href = "#/Page1";
            }
        }
    }
</script>
<style>
    .hidden { display: none; }
</style>
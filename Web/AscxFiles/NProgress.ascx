<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string Done { get; set; }

        [NProgress, Ready("hidden")]
        protected override void OnCreated()
        {
            System.Threading.Thread.Sleep(3000);

            this.Done = "Document done from server: " + DateTime.Now;
        }

        [NProgress, Loading("btn")]
        public void Wait(int time)
        {
            System.Threading.Thread.Sleep(time);
        }
    }

</script>
<template>
    <div class="hidden">
        <h3>Progress bar</h3><hr />
        {{ Done }}
        <hr />
        <button @click="Wait(2000)" ref="btn">Click and Wait</button>
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
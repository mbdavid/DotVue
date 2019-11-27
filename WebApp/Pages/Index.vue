@page / 

<div class="item">
    <h1>My Id Counter is {{ id }}</h1>
    <button type="button"
            @click="clickMe(5)">
        ClickMe-Server
    </button>
    <button type="button"
            @click="id++">
        ClickMe-Client
    </button>
    <hr/>
    <input v-model="text" />
    <hr/>
    {{text}} | Code: {{ code }}
    <hr/>
    <button @click="showCode()">ShowCode (with confirm)</button>
    <hr/>
</div>

<style scoped>

    button {
        color:red;
    }

</style>

<script>

    this.$on('showCode:before', function (r) {
        if (confirm('Submit?')) {
            r.resolve();
        }
        else {
            r.reject();
        }
    })

</script>
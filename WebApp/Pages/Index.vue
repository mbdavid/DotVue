@page / 
@async0

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
    <button @click="showCode()">ShowCode</button>
</div>

<style scoped>

    button {
        color:red;
    }

</style>

<script>

    this.$on('clickMe:after', function (r) {
        console.log('click after - result is ' + r)
    })

</script>
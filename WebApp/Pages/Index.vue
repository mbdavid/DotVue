@viewmodel
@page /
@ascync

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
    <input v-model="Text" />
</div>

<style scoped>

    button {
        color:red;
    }

</style>

<script>

    this.$on('clickMe:after', function () {
        console.log('click after')
    })

</script>
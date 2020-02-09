@page / 

<div>
    <counter :number="100"></counter>
    <hr/>
    <input v-model="text" />
    <hr/>
    {{text | upper }} | Code: {{ code }}
    <hr/>
    <button @click="showCode()">ShowCode (with confirm)</button>
    <hr/>
</div>
<style scoped>

    & {
        background-color: #f5f5f5;
        padding: 15px;
    }

    button {
        color:red;
    }

</style>

<script>

    this.$on('showCode:before', function (r) {
        r.wait();
        if (confirm('Submit?')) {
            r.resolve();
        }
        else {
            r.reject();
        }
    })

</script>
<script global>

    // global script block will be added as script before load components

    Vue.filter('upper', function (value) {
        return value.toString().toUpperCase();
    })

</script>
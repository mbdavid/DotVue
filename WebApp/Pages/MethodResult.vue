@page /method-result
<div>
    <p>Server methods returns value using `Promise` and can be get using `then`.</p>

    <button @click="loadItems()" ref="btn">Load Items</button>
</div>

<script>

    this.loadItems = function () {
        this.GetList(10).then()
            .then(r => alert('Result from server: ' + r));
    }

</script>
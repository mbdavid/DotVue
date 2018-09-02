@viewmodel WebApp.Components.MethodResult

<template>
    <div>
        <p>Server methods returns value using `Promise` and can be get using `then`.</p>

        <button @click="loadItems()" ref="btn">Load Items</button>
    </div>
</template>

<script>
    return {
        methods: {
            loadItems() {
                this.GetList(10)
                    .then(r => alert('Result from server: ' + r));
            }
        }
    };
</script>
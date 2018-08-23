@viewmodel WebApp.Components.NProgress

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
            finish: function () {
                this.$destroy();
                location.href = "#/Page1";
            }
        }
    };
</script>

<style>
    .hidden {
        display: none;
    }
</style>
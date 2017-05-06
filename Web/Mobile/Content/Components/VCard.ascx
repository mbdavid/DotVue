<%@ Control Language="C#" %>
<template>

    <div>
        <div v-if="title" class="card-title">{{ title }}</div>
        <div class="card">
            <slot></slot>
        </div>
    </div>

</template>

<script>

    return {
        props: ['title', 'box']
    }

</script>

<style lang="less">

    @import "/Mobile/Content/Less/mixins.less";

    .card-title {
        padding: 0.4rem @gutter;
        color: @mdc-grey-700;
        font-size: 90%;
    }

    .card {
        border-top: 1px solid @mdc-grey-200;
        border-bottom: 1px solid @mdc-grey-200;
        padding: @gutter @gutter;
        background-color: white;
    }

    .card-boxed {
        border-left: 1px solid @mdc-grey-200;
        border-right: 1px solid @mdc-grey-200;
    }

</style>
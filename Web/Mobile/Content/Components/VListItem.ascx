<%@ Control Language="C#" %>
<template>

    <div class="list-item" v-href="href">

        <div class="list-item-left" v-if="icon">
            <ui-icon :icon="icon" v-if="icon" />
        </div>

        <div class="list-item-center">
            <div>
                <slot></slot>
                <div v-if="$slots.detail" class="list-item-detail">
                    <slot name="detail"></slot>
                </div>
            </div>
        </div>

        <div class="list-item-right" v-if="$slots.right">
            <slot name="right"></slot>
        </div>

        <div class="list-item-right" v-if="href">
            <ui-icon icon="chevron_right" size="large" />
        </div>
        
    </div>

</template>

<script>

    return {
        props: ['icon', 'href']
    }

</script>

<style lang="less">

    @import "/Mobile/Content/Less/mixins.less";

    .list-item {
        border-top: 1px solid @mdc-grey-200;
        border-bottom: 1px solid @mdc-grey-200;
        padding: 1rem @gutter;
        display: flex;
        cursor: pointer;
        background-color: white;
    }

    .list-item + .list-item {
        border-top: none;
    }

    .list-item-detail {
        color: @mdc-grey-500;
        font-size: 80%;
        margin-top: 0.3rem;
    }

    .list-item-left {
        flex: 0 0 auto;
        margin-right: 1rem;
        display: flex;
        align-items: center;
    }

    .list-item-center {
        flex: 1 1 auto;
        position: relative;
        display: flex;
        align-items: center;
    }

    .list-item-right {
        text-align: right;
        flex: 0 0 auto;
        color: @mdc-grey-500;
        display: flex;
        align-items: center;
        font-size: 80%;
        align-content: center;
    }

    .card > .list-item {
        margin-left: -@gutter;
        margin-right: -@gutter;
    }

    .card > .list-item:first-child {
        margin-top: -@gutter;
        border-top: none;
    }

    .card > .list-item:last-child {
        margin-bottom: -@gutter;
        border-bottom: none;
    }

</style>
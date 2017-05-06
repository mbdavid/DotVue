<%@ Control Language="C#" %>
<template>

    <div class="page-app">

        <div class="page-fixed">
            <ui-toolbar :title="title" nav-icon="home" type="colored" text-color="white" @nav-icon-click="goHome()">
                <div slot="actions">
                    <slot name="actions"></slot>
                    <ui-icon-button v-show="back" color="white" icon="arrow_back" size="large" type="secondary" v-href="back" />
                </div>
            </ui-toolbar>
            <slot name="header"></slot>
        </div>

        <div class="page-content">
            <slot></slot>
        </div>

    </div>

</template>

<script>

    return {
        props: ['title', 'back'],
        methods: {
            goHome: function() {
                location.href = '#/Home';
            }
        }
    }

</script>

<style>

    @import "/Mobile/Content/Less/mixins.less";

    .page-app {
        display: flex;
        flex-direction: column;
        height: 100%;
    }

    .page-fixed {
        flex: 0 0 auto;
    }

    .page-content {
        flex: 1 1 auto;
        position: relative;
        margin-top: 2px;
        overflow-y: scroll; /* has to be scroll, not auto */
        -webkit-overflow-scrolling: touch;
    }

</style>
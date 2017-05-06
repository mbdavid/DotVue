<%@ Control Language="C#" %>
<template>

    <div class="pager">

        <ui-icon-button type="secondary" color="primary" icon="first_page" @click="current = 1" :disabled="current == 1" />
        <ui-icon-button type="secondary" color="primary" icon="chevron_left" @click="current--" :disabled="current == 1" />

        <span>
            {{ current }} / {{ pages }}
        </span>

        <ui-icon-button type="secondary" color="primary" icon="chevron_right" @click="current++" :disabled="current == pages" />
        <ui-icon-button type="secondary" color="primary" icon="last_page" @click="current = pages" :disabled="current == pages" />
        
    </div>

</template>

<script>

    return {
        props: ['current', 'pages'],
        watch: {
            current: function(v) {
                this.$emit('move-page', v);
            }
        }
    }

</script>

<style lang="less">

    @import "/Mobile/Content/Less/mixins.less";

    .pager {
        border-bottom: 1px solid @mdc-grey-200;
        padding: 1rem;
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: white;
    }

    .pager > * {
        margin: 0 @gutter;
    }

    .card > .pager:last-child {
        margin-bottom: -@gutter;
    }

</style>
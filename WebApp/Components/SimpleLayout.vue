@viewmodel WebApp.Components.SimpleLayout

<template>
    <div class="simple-layout">
        <h1>Simple Layout</h1>
        <hr />
        <div class="toolbar">
            <button @click="$emit('save')" :disabled="!SaveEnabled">Save</button>
            <button @click="LocalTest()">Test if save are enabled</button>
            <span class="slot">
                <slot name="toolbar"></slot>
            </span>
        </div>
        <hr />
        <form class="slot">
            <slot name="form"></slot>
        </form>
    </div>
</template>

<style>
    @import url("/base.css");

    .simple-layout {
        border: 2px solid var(--danger);
        margin: var(--gutter);
        padding: var(--gutter);
        background-color: var(--red-50);
    }
    .simple-layout .slot {
        padding: var(--gutter);
        background-color:var(--yellow-A100);
        border: 1px solid var(--danger);
    }
</style>
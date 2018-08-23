@viewmodel WebApp.Components.FormInner

<template>
    <div style="border: 1px solid blue; padding: 20px;">
        <h3>Inner Form</h3>
        <hr />
        <input type="text" v-model="Id" placeholder="Id value" />

        <button type="button" @click="SaveClick()">Save on Server (inner form)</button>

    </div>
</template>

@viewmodel WebApp.Components.LayoutContent

<template>
    <simple-layout @save="Save()" :save-enabled="IsSaveEnabled">

        <template slot="toolbar">
            <button @click="IsSaveEnabled = !IsSaveEnabled">Toggle</button>
        </template>

        <template slot="form">
            This your content inside
            <b>simple-layout</b>
            <br />
            Save options is: {{ IsSaveEnabled }}
        </template>

    </simple-layout>

</template>


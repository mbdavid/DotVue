@viewmodel WebApp.Components.QueryString

<template>
    <div class="page2">
        <h4>Page 2</h4><hr />
        <div>
            [<a href="#/QueryString">Empty QueryString</a>]
            [<a href="#/QueryString?id=123">?id=123</a>]
            [<a href="#/QueryString?id=ABC&r=4455">?id=ABC&r=4455</a>]
        </div>

        <hr/>

        <button @click="ShowQuery()">ShowQuery()</button>
        <button @click="Inc()">{{ Counter }}</button>
        <pre>$data: {{ $data }}</pre>
        <pre>$props: {{ $props }}</pre>
    </div>
</template>

<style>
    .page2 h4 {
        color: blue;
    }
</style>
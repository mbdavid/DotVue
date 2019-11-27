@page /query-string
<div class="page2">
    <h4>QueryString Page</h4><hr />
    <div>
        [<router-link to="/query-string">Empty QueryString</router-link>]
        [<router-link to="/query-string?id=123">?id=123</router-link>]
        [<router-link to="/query-string?id=ABC&r=4455">?id=ABC&r=4455</router-link>]
    </div>

    <hr/>

    <button @click="showQuery()">ShowQuery()</button>
    <button @click="inc()">{{ counter }}</button>
    <pre>$data: {{ $data }}</pre>
</div>

<style>
    .page2 h4 {
        color: blue;
    }
</style>
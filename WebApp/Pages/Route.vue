@page /route/:id

<div>
    <h4>Route/QueryString Page</h4><hr />
    <div>
        [<router-link to="/route/100">Route/100</router-link>]
        [<router-link to="/route/200?page=123">Route/200?page=123</router-link>]
    </div>

    <button @click="showData()">ShowQuery()</button>
    <pre>$data: {{ $data }}</pre>

</div>
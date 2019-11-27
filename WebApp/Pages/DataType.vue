@page /data-type
<div>

    <h3>DateType</h3><hr />

    <input v-model.number="decimalValue" type="text" lang="pt-br" /><br />
    <input v-model="boolValue" type="checkbox" /><br />
    <input v-model="dateValue" type="date" /><br />
    <input v-model="inner.newDate" type="date" /><br />

    <button @click="post()">Post</button>

    <hr />
    <pre>{{ $data }}</pre>

</div>

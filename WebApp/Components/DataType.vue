@viewmodel WebApp.Components.DataType

<template>
    <div>

        <h3>DateType</h3><hr />

        <input v-model.number="DecimalValue" type="text" lang="pt-br" /><br />
        <input v-model="BoolValue" type="checkbox" /><br />
        <input v-model="DateValue" type="date" /><br />
        <input v-model="Inner.NewDate" type="date" /><br />

        <button @click="Post()">Post</button>

        <hr />
        <pre>{{ $data }}</pre>

    </div>
</template>

@viewmodel WebApp.Components.Local

<template>
    <div>
        [Local] properties only send data from server to client. Useful for grid models
        <hr/>
        <button @click="ChangeData()">ChangeData()</button>
        <button @click="CheckData()">CheckData()</button>
        <hr />
        <ul><li v-for="i in Items">{{ i }}</li></ul>
        <hr />
        <pre>{{ $data }}</pre>
    </div>
</template>
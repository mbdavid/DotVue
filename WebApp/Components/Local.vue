@viewmodel WebApp.Components.Local

<template>
    <div>
        <button @click="LoadData()">LoadData()</button>
        <button @click="ChangeData()">ChangeData()</button>
        <button @click="CheckData()">CheckData()</button>
        <hr />
        <ul><li v-for="i in Items">{{ i }}</li></ul>
        <hr />
        <pre>{{ $data }}</pre>
    </div>
</template>
@viewmodel WebApp.Components.LoginVM
@auth
@role admin

<template>
    <div class="item">
        <h1>Must be logged to see this page</h1>
        <h2>Must have "admin" role also</h2>
        <hr/>
        <button @click="ServerClick()">Must have 'click' role to click here</button>
    </div>
</template>

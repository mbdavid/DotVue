@viewmodel WebApp.Components.Page1

<template>
    <div class="item">
        <h1>My Id Counter is {{ Id }}</h1>
        <button type="button"
                @click="ClickMe(5)">
            ClickMe-Server
        </button>
        <button type="button"
                @click="Id++">
            ClickMe-Client
        </button>
    </div>
</template>

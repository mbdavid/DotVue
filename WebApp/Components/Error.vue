@viewmodel WebApp.Components.Error

<template>
    <div>
        <button @click="ClickError(1, 2, 'nine')">Error in server</button>
		<br/>
        <button @click="NeedAuthentication()">Need authentication</button>
		<br/>
        <button @click="NeedAuthorization()">Need authorization</button>


    </div>
</template>
@viewmodel WebApp.Components.Controls

<template>

    <div>
        <p>Autcomplete:</p>

        <n-auto-complete prop="Name" v-model="Selected" :items="CountriesResult" @query="QueryData" placeholder="Countries">
            <template scope="s">
                <div style="float: left;">{{ s.item.Name }}</div>
                <div style="float: right">{{ s.item.Code }}</div>
            </template>
        </n-auto-complete>


        <br />Selected:<br />
        <pre style="border: 1px solid blue;">{{ Selected }}</pre>

    </div>


</template>
@ViewModel Web.VueFiles.BtnCounter
<template>
    <button @click="IncrementNumber(1)" class="counter hidden" ref="btn">
        Vue: {{ Number }}
    </button>
</template>
<style>
    button:disabled { color: silver; }
    .counter { color: blue }
    .hidden { display: none; }
</style>
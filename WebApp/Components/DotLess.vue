<template>
    <div class="less">
        <h1>Using Less compiler (blue)</h1>
        <h2>Using Less compiler with include support (red)</h2>
    </div>
</template>

<style lang="less" import="/base.less">

    @blue: blue;

    .less h1 {
        color: @blue;
    }

    /*
    .less h2 {
        color: @red;
    }
    */

</style>

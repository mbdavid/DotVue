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
<script global>

    Vue.directive('model-date', {
        bind: function (el, binding, vnode) {
            /*
            var s = JSON.stringify
            el.innerHTML =
              'name: '       + s(binding.name) + '<br>' +
              'value: '      + s(binding.value) + '<br>' +
              'expression: ' + s(binding.expression) + '<br>' +
              'argument: '   + s(binding.arg) + '<br>' +
              'modifiers: '  + s(binding.modifiers) + '<br>' +
              'vnode keys: ' + Object.keys(vnode).join(', ')*/


        }
    })


</script>
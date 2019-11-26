@page /page1/:qtd

<div>
    <h1>Page 1</h1>
    Params qtd: {{ qtd }} - QueryString p: {{ p }}
    <hr />
    <button @click="saveThis()">ClickMe</button>
    <button @click="errorThis()">ErrorMe</button>

</div>
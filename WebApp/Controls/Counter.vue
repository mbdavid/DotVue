<div>

    <h1>My Id Counter is {{ number }}</h1>
    <button type="button"
            @click="serverClick()">
        ClickMe-Server
    </button>
    <button type="button"
            @click="number++">
        ClickMe-Client
    </button>

</div>
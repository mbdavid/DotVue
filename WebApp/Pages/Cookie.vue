@page /cookie
    <div>
        <h1>Cookie Example - Server/Client</h1>
        <hr />
        <input type="text" v-model="value" />

        <button @click="readCookie()">Read Cookie</button>
        <button @click="updateCookie()">Update Cookie</button>
    </div>

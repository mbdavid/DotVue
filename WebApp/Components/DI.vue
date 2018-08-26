@viewmodel WebApp.Components.DI

<template>
    <div>
        <h2>From Ctor</h2>
        <table border="1">
            <tr>
                <td>Transient</td>
                <td>{{ Transient }}</td>
            </tr>
            <tr>
                <td>Scoped </td>
                <td>{{ Scoped }}</td>
            </tr>
            <tr>
                <td>Singleton</td>
                <td>{{ Singleton }}</td>
            </tr>
            <tr>
                <td>SingletonInstance</td>
                <td>{{ SingletonInstance }}</td>
            </tr>
        </table>
        <hr />

        <h2>From Service</h2>
        <table border="1">
            <tr>
                <td>Transient</td>
                <td>{{ Transient2 }}</td>
            </tr>
            <tr>
                <td>Scoped </td>
                <td>{{ Scoped2 }}</td>
            </tr>
            <tr>
                <td>Singleton</td>
                <td>{{ Singleton2 }}</td>
            </tr>
            <tr>
                <td>SingletonInstance</td>
                <td>{{ SingletonInstance2 }}</td>
            </tr>
        </table>

        <h2>From IServiceProvider</h2>
        <table border="1">
            <tr>
                <td>Transient</td>
                <td>{{ Transient3 }}</td>
            </tr>
            <tr>
                <td>Scoped </td>
                <td>{{ Scoped3 }}</td>
            </tr>
            <tr>
                <td>Singleton</td>
                <td>{{ Singleton3 }}</td>
            </tr>
            <tr>
                <td>SingletonInstance</td>
                <td>{{ SingletonInstance3 }}</td>
            </tr>
        </table>

        <hr/>
        <button @click="OnCreated()">New Request</button>

    </div>
</template>
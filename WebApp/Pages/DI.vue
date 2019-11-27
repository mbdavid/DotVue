@page /di
<div>
    <h2>From Ctor</h2>
    <table border="1">
        <tr>
            <td>Transient</td>
            <td>{{ transient }}</td>
        </tr>
        <tr>
            <td>Scoped </td>
            <td>{{ scoped }}</td>
        </tr>
        <tr>
            <td>Singleton</td>
            <td>{{ singleton }}</td>
        </tr>
        <tr>
            <td>SingletonInstance</td>
            <td>{{ singletonInstance }}</td>
        </tr>
    </table>
    <hr />

    <h2>From Service</h2>
    <table border="1">
        <tr>
            <td>Transient</td>
            <td>{{ transient2 }}</td>
        </tr>
        <tr>
            <td>Scoped </td>
            <td>{{ scoped2 }}</td>
        </tr>
        <tr>
            <td>Singleton</td>
            <td>{{ singleton2 }}</td>
        </tr>
        <tr>
            <td>SingletonInstance</td>
            <td>{{ singletonInstance2 }}</td>
        </tr>
    </table>

    <h2>From IServiceProvider</h2>
    <table border="1">
        <tr>
            <td>Transient</td>
            <td>{{ transient3 }}</td>
        </tr>
        <tr>
            <td>Scoped </td>
            <td>{{ scoped3 }}</td>
        </tr>
        <tr>
            <td>Singleton</td>
            <td>{{ singleton3 }}</td>
        </tr>
        <tr>
            <td>SingletonInstance</td>
            <td>{{ singletonInstance3 }}</td>
        </tr>
    </table>

    <hr/>
    <button @click="onCreated()">New Request</button>

</div>
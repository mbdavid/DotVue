@viewmodel WebApp.Components.Includes
@inheritattrs false

<!-- Dependencies -->
<link href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO" crossorigin="anonymous">
<script src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
<script src="/demo.js"></script>

<template>
    <div>
        <h1>Bootstrap was loaded</h1>
        <button type="button" class="btn btn-primary">Primary</button>
        <button type="button" class="btn btn-secondary">Secondary</button>
        <button type="button" class="btn btn-success">Success</button>
        <button type="button" class="btn btn-danger">Danger</button>
        <button type="button" class="btn btn-warning">Warning</button>
        <button type="button" class="btn btn-info">Info</button>
        <button type="button" class="btn btn-light">Light</button>
        <button type="button" class="btn btn-dark">Dark</button>
        <hr />
        <h2>An jQuery too</h2>
        <button @click="hasJquery()">Test $</button>
    </div>
</template>


<script>

    // using jquery before load vue instance
    $('body').css('color', 'gray');

    return {
        methods: {
            hasJquery() {
                alert("$ = " + typeof $);
            }
        }
    };

</script>
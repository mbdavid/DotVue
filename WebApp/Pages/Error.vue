@page /error
<div>
    <button @click="clickError(1, 2, 'nine')">Error in server</button>
	<br/>
    <button @click="needAuthentication()">Need authentication</button>
	<br/>
    <button @click="needAuthorization()">Need authorization</button>


</div>
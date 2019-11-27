@page /form-emit
<div>
    <h3>Form - Using event "Emit" method to comunicate between parent-child components</h3>
    <hr />

    <form-inner @onsave="save"></form-inner>

    <div>
        If you click on "Save on Server", this child component will trigger (emit) event to parent component (this). This event are captured by '@onsave' attribute and run server code (with parameter support)
    </div>

</div>

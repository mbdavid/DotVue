<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public DateTime DateValue { get; set; } = DateTime.Now;
        public bool BoolValue { get; set; } = false;
        public decimal DecimalValue { get; set; } = 1.99m;
        public Inner Inner = new Inner();

        public void Post()
        {
            this.DateValue = DateValue.AddYears(-1);
            this.DecimalValue = -DecimalValue;
            this.BoolValue = !BoolValue;
            this.Inner.NewDate = Inner.NewDate.AddYears(1);
        }
    }

    public class Inner
    {
        public DateTime NewDate { get; set; } = new DateTime(2000, 1, 1);
    }

</script>
<template>
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
</template>

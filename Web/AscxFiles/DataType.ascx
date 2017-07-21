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
            DateValue = DateValue.AddYears(-1);
            DecimalValue = -DecimalValue;
            BoolValue = !BoolValue;
            Inner.NewDate = Inner.NewDate.AddYears(1);
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

        <input v-model.number="DecimalValue" type="text" /><br />
        <input v-model="BoolValue" type="checkbox" /><br />
        <input v-model="DateValue" type="date" /><br />

        <button @click="Post()">Post</button>

        <hr />
        <pre>{{ $data }}</pre>

    </div>
</template>

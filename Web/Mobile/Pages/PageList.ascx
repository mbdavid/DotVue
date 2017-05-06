<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public int Counter { get; set; }
        public DateTime? Ini { get; set; } = DateTime.Now;
        public DateTime? Fim { get; set; }
        public List<string> Items { get; set; } = new List<string>();
        public string Selected { get; set; } = "Blue";

        public List<string> Records { get; set; }

        public void AddYear()
        {
            JS.CloseModal("FilterModal");
            Records = new List<string> { "Item 1", "Item 2", "Item 3" };
        }
    }

</script>

<template>
<v-page-layout title="List Page" back="#/Home">

    <ui-icon-button slot="actions" icon="search" @click="$refs.FilterModal.open()" color="white" size="large" type="secondary" />

    <ui-modal ref="FilterModal" title="Filter">

        <ui-textbox label="Name" />

        <div class="left-right">
            <ui-datepicker v-model="Ini" label="Initial" picker-type="modal" />
            <ui-datepicker v-model="Fim" label="Final" picker-type="modal" />
        </div>

        <ui-select label="Options" :options="['Red', 'Blue', 'Green']" v-model="Items" :has-search="true" :multiple="true" />

        <ui-checkbox-group v-model="Items" :options="['Red', 'Blue', 'Green']" label="Select colors" />
        <br />

        <ui-button icon="search" @click="AddYear()" color="primary">Search</ui-button>

    </ui-modal>

    <v-list-item v-for="item in Records" :href="'#/EditPage?Id=' + item">
        Open record: {{ item }}
    </v-list-item>

</v-page-layout>
</template>
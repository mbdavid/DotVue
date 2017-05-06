<%@ Control Language="C#" %>
<template>
<v-page-layout title="Home">

    <v-list-item href="#/FilterPage">Filter Page</v-list-item>
    <v-list-item icon="bluetooth_searching">No link</v-list-item>

    <v-list-item icon="bluetooth_searching" href="#/FilterPage">
        No link
        <div slot="detail">Lorem lipsum dag lipson mlusp lorems hris jups msl. Losrme, mshro, masd ehjs asdli</div>
        <div slot="right">USD 19.900,00<br />USD 300,00</div>
    </v-list-item>

    <v-list-item icon="bluetooth_searching">
        With buttons
        <ui-button slot="right" icon="mode_edit" color="primary" type="secondary" size="small">Edit</ui-button>
        <ui-button slot="right" icon="close" color="red" type="secondary" size="small">Delete</ui-button>
    </v-list-item>

    <v-list-item icon="bluetooth_searching">
        With checkbox
        <ui-checkbox slot="right" />
    </v-list-item>

    <v-list-item v-for="i in 5" icon="home" href="#/NewPage">
        Description
        <div slot="detail">Small description about this link</div>
    </v-list-item>
    <v-list-item v-for="i in 20" icon="person_pin" href="#/NewPage">
        Go to another page {{ i }}
    </v-list-item>

    <v-pager current="4" pages="12" />

</v-page-layout>
</template>
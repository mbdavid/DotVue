<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string Timer { get; set; }

        [Confirm("Confirm logout?")]
        public void Logout()
        {
            var cookie = HttpContext.Current.Request.Cookies["access_token"];
            HttpContext.Current.Response.Cookies.Remove("access_token");
            cookie.Expires = DateTime.Now.AddDays(-10);
            cookie.Value = null;
            HttpContext.Current.Response.SetCookie(cookie);
            JS.Code("location.reload();");
        }

        [Loading("wait")]
        public void Wait(int ms)
        {
            System.Threading.Thread.Sleep(ms);

            Timer = " :: " + DateTime.Now.ToString();
        }
    }

</script>

<template>
<v-page-layout title="Home">

    <ui-icon-button slot="actions" icon="exit_to_app" @click="Logout()" color="white" size="large" type="secondary" />

    <p class="margin">Welcome to MyApp mobile framework in VueJS using vue-server api. Click in any item below:</p>

    <v-card title="Applications">
        <v-list-item href="#/PageList">List Page</v-list-item>
        <v-list-item href="#/PageNew">New Page</v-list-item>
        <v-list-item href="#/PageView">View Page</v-list-item>
        <v-list-item href="#/ListControl">List Control</v-list-item>

        <v-pager current="3" pages="3" />

    </v-card>

    <br /><br />

    <ui-button ref="wait" @click="Wait(2000)" color="primary">Waiting button{{ Timer }}</ui-button>

</v-page-layout>
</template>
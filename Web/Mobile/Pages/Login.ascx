<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public void Login()
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("access_token", Username));

            JS.Reload();
        }
    }

</script>

<template>

    <div class="login-box">
        <h1>MyApp Login</h1>
        <ui-textbox v-model="Username" label="Username" />
        <ui-textbox v-model="Password" label="Password" type="password" />
        <div style="margin-top: 2rem;">
            <ui-button @click="Login()" color="primary" size="large" :raised="true">Login</ui-button>
            <span class="help">
                Use any username/password
            </span>
        </div>
    </div>

</template>

<style lang="less">

    @import "/Mobile/Content/Less/mixins.less";

    .login-box {
        margin: 2rem;
        padding: 1rem;
        border: 1px solid @mdc-blue-grey-200;
        background-color: white;
        .box-shadow(5px, 5px, 3px, 0.1);
    }

    .login-box h1 {
        color: @mdc-grey-800;
        margin-top: 0;
        text-align: center;
    }

    .login-box .help {
        color: @mdc-grey-400;
        float: right;
        font: 90%;
        font-style: italic;
    }

</style>
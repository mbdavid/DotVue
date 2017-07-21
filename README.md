# DotVue

Implement `.vue` single-file component with server-side ViewModel using `.ascx` file.


```HTML
<script runat="server">

    public class LoginVM : ViewModel
    {
        public Username { get; set; }
        public Password { get; set; }
        public Message { get; set; }

        public void Login()
        {
            Message = AuthServie.Login(Username, Password);
        }
    }
    
</script>

<template>

    <div class="login-box">
        <p>
            <label>Username</label>
            <input type="text" v-model="Username" />
        </p>
        <p>
            <label>Password</label>
            <input type="password" v-model="Password" />
        </p>
        <p>
            <button @click="Login()">Login</button>
            <a @click="Clear()">Clear</a>
        </p>
        <div v-show="Message" class="alert">{{ Message }}</div>
    </div>
    
</template>

<style lang="less">

    /* Support less tags */
    @import "base.less";
    
    .login-box {
        border: 1px solid @line-color;
        button { display: block; }
    }
    
</style>

<script>

    // Optional: add Vue mixin (client only)
    return {
        methods: {
            Clear: function() {
               this.Username = "";
               this.Password = "";
               this.Message = "";
            }
        }
    }
    
</script>
```

## Setup

- Add `.vue` handler
```XML
    <handlers>
      <add name="vue" path="*.vue" type="DotVue.Handler, DotVue" verb="*"/>
    </handlers>
```
- Add in your page script for `bootstrap.vue` and with discover all components: sync (inline) or async (via ajax loader)
```HTML
    <script src="bootstrap.vue?discover=async"></script>
```

- Write you own .ascx component as Vue single component

# Features

- Server based ViewModel with attributes decorations: methods, watchs and props
- Support file upload
- Resolve computed server-side expression to javascript function (using simple LINQ visitor)
- Support multiple loaders: `.ascx`, `.vue` or static file to render as single javascript Vue component
- Bootstrap and discover all components
- Support custom tag compiler (like LESS or TypeScript)
- Ready for any Vue plugin

# TODO

- Add more control over Component
    - JObject as model
    - Template
    - Scripts, Styles (many tags)
    - Methods, Watchs, Pre, Pos...
    - Name
    - *Plugins* (add in Loader ways to auto-add?)
    
- Support for plugins
    Plugins are a simple way to extend a component without touch on original component. Possible to add methods, scripts and section inside template (when template offers %section% to recive more HTML)
    - Section %form1%, %toolbar%
    - Plugin class pipe-line on execution
    - Load plugins by same ILoader
    - Check plugin inside Plugin by override method `bool Install(string name)`
    
- Support for mobile/tablet templates/scripts/styles
    - <template target="mobile"> (mobile, tablet, desktop)

- Support "import/require" in <script> 
    <script src="http://cdn.net/jquery.js" var="$" /> 
    
- Cache discover + loader

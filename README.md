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
        <label>Username</label>
        <input type="text" v-model="Username" />
        <label>Password</label>
        <input type="password" v-model="Password" />
        <button @click="Login()">Login</button>
        <a @click="Clear()">Clear</a>
        <div v-show="Message" class="alert">{{ Message }}</div>
    </div>
</template>

<style lang="less">
    @import "base.less";
    .login-box {
        border: 1px solid @line-color;
        button { display: block; }
        .alert { ... }
    }
</style>

<script>
    // Vue mixin (client only)
    return {
        methods: {
            Clear: function() {
               this.Username = "";
               this.Password = "";
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
- Add in your page script for `bootstrap.vue` and with discover all components
```HTML
    <script src="bootstrap.vue?discover=async"></script>
```

- Write you own .ascx component as Vue single component


# Features

- Server based ViewModel with attributes decorations: methods, watchs and props
- Support file upload
- `C#` DateTime to `Date()` javascript object
- Resolve computed server-side expression to javascript function (using simple LINQ visitor)
- Support multiple loaders: `.ascx`, `.vue` or static file to render as single javascript Vue component
- Bootstrap and discover all components
- Support custom tag compiler (like LESS or TypeScript)
- Ready for any Vue plugin

# TODO

- Support 2 ways to client script:
```
    <script> (render before vue {..}, to create custom methods)
    <script mixin> (x1)
```   
   
- VueList<T> : IList<T> (capture changes)    
    - Add(T item)
    - Set(int index, Action<T> fn)
    - Remove(int index)
    - Length
    
- Support to external "src" in tags
- Support multiple loader() with orders
- Cache discover + Load before

- StaticLoader

StaticLoader.RegisterComponent("NButton", typeof(NButtonVM), Stream content);




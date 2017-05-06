# DotVue

Implement `.vue` single-file component with server-side ViewModel in a `.ascx` file.


```C#
<%@ Control Language="C#" %>
<script runat="server">

    public class ComponentVM : ViewModel
    {
        public Username { get; set; }
        public Password { get; set; }
        public Message { get; set; }

        public void Login()
        {
            Message = AuthServie.Login(Username, Password);
        }
        
        public void Username_Watch(string value, string old)
        {
            // observe changes from client
        }
    }
    
</script>
<template>
    <div>
        Username: <input type="text" v-model.lazy="Username" /><br/>
        Password: <input type="password" v-model="Password" /><br/>
        <button type="button" v-on:click="Login()">Login</button>
        <hr/>
        {{ Message }}
    </div>
</template>
<style>
    .my-button { ... }
</style>
<script>
    // vue mixin
    return {
        methods: {
            showError: function() {
               // ..
            }
        }
    }

</script>
```

# TODO

- Support 3 different ways to script:
```
    <script> (xN)
    <script src=".."> (xN)
    <script mixin> (x1)
```   
   
- VueList<T> : IList<T> (capture changes)    
    - Add(T item)
    - Set(int index, Action<T> fn)
    - Remove(int index)
    - Length
    
- Support to external "src" in tags
- Support custom ScriptRender e ComponentExecution

- Support to custom tags. This tags can be readed when?

<doc>
    # Implement documents
    - In markdown format
    - Or any other format
<doc>    
<resource>
    read - Read data
    write - Write data
    execute - Permition to execute some operation
</resource>
<menu>
    <name>Items selecionavel</name>
    <app>XPS</app>
</menu>

# DotVue

Implement `.vue` single-file component with server-side ViewModel. Use all power of VueJS with simple C# server side data access.

> Login.vue.cs

```C#
namespace ServerViewModel
{
    public class Login : ViewModel
    {
        public Username { get; set; }
        public Password { get; set; }
        
        public Message { get; set; }

        public void Login()
        {
            this.Message = AuthServie.Login(Username, Password);
        }
    }
}
```

> Login.vue

```HTML
@viewmodel ServerViewModel.Login

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

<style>

    .login-box {
        border: 1px solid silver;
        button { display: block; }
    }
    
    .alert {
        color: red;
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

```C#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseDotVue(c =>
    {
        c.AddAssembly(typeof(Startup).Assembly);
    });
}
```

# Features

- ASP.NET Core 2
- Server based ViewModel with attributes decorations: methods, watchs and props
- Deploy `.vue` file as embedded resource (deploy only `.dll` file)
- Support file upload
- Support any external vue plugin

- See `WebApp` for examples



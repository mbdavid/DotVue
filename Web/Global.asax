<%@ Application Language="C#" %>
<script runat="server">

    protected void Application_Start(object sender, EventArgs e)
    {
        DotVue.Handler.Loaders.Add(new DotVue.VueLoader());

        DotVue.StaticLoader.RegisterComponent("RedButton", "<template><button style='background-color:red'><slot/></button></template>");

        DotVue.Component.RegisterCompiler("style", "less", 
            (s) => dotless.Core.LessWeb.Parse(s, new dotless.Core.configuration.DotlessConfiguration { Web = true, MinifyOutput = true }));
    }

</script>

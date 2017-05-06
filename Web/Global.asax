<%@ Application Language="C#" %>
<script runat="server">

    protected void Application_Start(object sender, EventArgs e)
    {
        //DotVue.Handler.Loader = new DotVue.VueLoader();

        DotVue.Component.RegisterCompiler("style", "less", 
            (s) => dotless.Core.LessWeb.Parse(s, new dotless.Core.configuration.DotlessConfiguration { Web = true, MinifyOutput = true }));
    }

</script>

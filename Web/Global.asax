<%@ Application Language="C#" %>
<script runat="server">

    protected void Application_Start(object sender, EventArgs e)
    {
        DotVue.Component.Setup(c =>
        {
            c.Compilers["less"] =
                s => dotless.Core.LessWeb.Parse(s, new dotless.Core.configuration.DotlessConfiguration { Web = true, MinifyOutput = true });

        });
    }

</script>

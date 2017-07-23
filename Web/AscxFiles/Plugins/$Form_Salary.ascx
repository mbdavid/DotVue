<%@ Control Language="C#" %>
<script runat="server">

    public class PluginVM : ViewModel
    {
        public decimal Salary { get; set; } = 100.0m;

        public void Save()
        {
            ClientScript.Alert("Save salary " + Salary + " to Id = " + Data["Id"]);
        }
    }

</script>

<content for="form">
    <tr>
        <td>Salary (plugin)</td>
        <td>$ <input type="text" v-model.number="Salary" size="5" /></td>
    </tr>
</content>

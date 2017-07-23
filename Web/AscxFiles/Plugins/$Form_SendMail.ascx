<%@ Control Language="C#" %>
<script runat="server">

    public class PluginVM : ViewModel
    {
        public bool SendEmail { get; set; } = true;

        public void Save()
        {
            if (this.SendEmail)
            {
                ClientScript.Alert("Send email from plugin");
            }
        }

        public void Load()
        {
            Data["Name"] = "John Doe";
            Data["Age"] = 35;
        }
    }

</script>

<content for="toolbar">
    <button @click="Load()">Load from plugin</button>
</content>

<content for="form">
    <tr class="silver">
        <td colspan="2">
            <label>
                <input type="checkbox" v-model="SendEmail" />
                Send email when save (from plugin)
            </label>
        </td>
    </tr>
</content>

<style>
    .silver { background-color: #d3d3d3; }
</style>
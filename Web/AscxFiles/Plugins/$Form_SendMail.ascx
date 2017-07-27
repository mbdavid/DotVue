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

<template for="toolbar">
    <button @click="Load()">Load from plugin</button>
</template>

<template for="form">
    <tr class="silver">
        <td colspan="2">
            <label>
                <input type="checkbox" v-model="SendEmail" />
                Send email when save (from plugin)
            </label>
        </td>
    </tr>
</template>

<style>
    .silver { background-color: #d3d3d3; }
</style>
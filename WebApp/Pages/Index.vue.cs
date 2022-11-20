using DotVue;

namespace WebApp.Pages
{
    public class Index : ViewModel
    {
        public int Id { get; set; } = 3;
        public string Text { get; set; } = "Initial value";

        [Prop]
        public string Code = "ok";

        public void ClickMe(int number)
        {
            Id += number;
        }

        public void SubmitWithConfirm()
        {
            ClientScript.Code("this.$router.push('todo-client');");
        }

        public void Text_Watch(string newValue, string oldValue)
        {
            // triggered when "Text" changes on the client
        }

        public void ServerNavigate()
        {
            ClientScript.NavigateTo("route/456?yy=zz");
        }

        public void ServerExecute()
        {
            ClientScript.Call("serverExecuted", new[] { "Value1", "Value2" });
        }
    }
}

using DotVue;
using System.Collections.Generic;

namespace WebApp.Pages
{
    public class Index : ViewModel
    {
        public int Id { get; set; } = 3;
        public string Text { get; set; } = "Initial value";
        [Local]
        public string LocalOnly { get; set; } = "Client only data, does not get sent to the server.";
        [Local]
        public RequestResult Result { get; set; }

        [Prop]
        public string Code = "ok";

        public Index()
        {
            Result = new RequestResult();
        }

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

            Result.Message = "A result message.";
            Result.Success = true;
        }
    }

    public class RequestResult
    {
        public bool? Success { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> ResponseData { get; set; } = new Dictionary<string, object>();
    }
}

using DotVue;
using System;

namespace WebApp.Components
{
    [Autorize]
    public class LoginVM : ViewModel
    {
        [Autorize("click")]
        public void ServerClick()
        {
            this.ClientScript.Alert("clicked");
        }
    }
}

using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Components
{
    public class IoC : ViewModel
    {
        private IEmail _email;

        public IoC(IEmail email)
        {
            _email = email;
        }

        public void SendEmail()
        {
            var id = _email.Send("john@gmail.com", "demo1", "body email");

            this.ClientScript.Alert("Message send (see VS debug) - Return: " + id);
        }
    }
}
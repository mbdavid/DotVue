using DotVue;
using System;

namespace WebApp.Components
{
    public class EncodingVM : ViewModel
    {
        public string Text { get; set; }

        public void FromServer()
        {
            this.Text = "Hèllo Wôrld - Mãçâ";

            this.ClientScript.Alert(this.Text);
        }
    }
}

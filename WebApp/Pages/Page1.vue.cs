using DotVue;
using System;

namespace WebApp.Pages
{
    public class Page1 : ViewModel
    {
        [RouteParam]
        public int Qtd = 15;

        [QueryString]
        public int P = 25;

        public void SaveThis()
        {
            ClientScript.Alert($"RouteParam = {Qtd} - QueryString = {P}");
        }

        public void ErrorThis()
        {
            throw new Exception("Server error message");
        }
    }
}

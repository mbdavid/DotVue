using DotVue;
using System;

namespace WebApp.Pages
{
    public class Page1 : ViewModel
    {
        [RouteParam]
        public int Qtd = 0;

        [QueryString]
        public int P = 25;
    }
}

using DotVue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Controls
{
    public class Counter : ViewModel
    {
        [Prop]
        public int Number;

        public void ServerClick()
        {
            this.Number++;
        }

    }
}
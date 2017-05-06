using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotVue;

namespace Web.VueFiles
{
    public class BtnCounter : ViewModel
    {
        public int? Number { get; set; }

        [Ready]
        protected override void OnCreated()
        {
            System.Threading.Thread.Sleep(1000);
            Number = DateTime.Now.Second;
        }

        [Loading("btn")]
        public void IncrementNumber(int seq)
        {
            System.Threading.Thread.Sleep(1000);
            Number++;
            JS.Emit("inc", seq);
        }
    }
}